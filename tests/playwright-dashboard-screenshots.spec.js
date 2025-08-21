// tests/playwright-dashboard-screenshots.spec.js
// Playwright test file for capturing Aspire Dashboard and Frontend screenshots

const { test } = require('@playwright/test');
const fs = require('fs');

// Ignorar errores de certificados locales (https://localhost)
test.use({ ignoreHTTPSErrors: true });

const DASHBOARD_CANDIDATES = [
  'https://localhost:17074', // visto en salida previa
  'http://localhost:15040',  // puerto http del AppHost según launchSettings
  'https://localhost:17187', // valor por defecto de ejemplo del prompt
  'https://localhost:17070',
  'https://localhost:17071',
  'https://localhost:17072',
  'https://localhost:17073',
  'https://localhost:17075'
];

const FRONTEND_CANDIDATES = [
  'https://localhost:7206', // permitido en CORS de la API
  'http://localhost:5274',  // permitido en CORS de la API
  'https://localhost:5001',
  'http://localhost:5000'
];

const SCREENSHOTS_DIR = 'docs/screenshots';

async function resolveUrl(page, candidates, preferred) {
  if (preferred) {
    try {
      await page.goto(preferred, { waitUntil: 'domcontentloaded', timeout: 8000 });
      await page.waitForTimeout(300);
      return preferred;
    } catch {
      // fallback to candidates
    }
  }
  for (const url of candidates) {
    try {
  await page.goto(url, { waitUntil: 'domcontentloaded', timeout: 8000 });
  // pequeña espera para estabilizar navegación inicial/redirecciones
  await page.waitForTimeout(300);
      return url;
    } catch {
      // probar siguiente
    }
  }
  return null;
}

function ensureDir(dir) {
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true });
  }
}

test('Captura Aspire Dashboard', async ({ page }) => {
  ensureDir(SCREENSHOTS_DIR);
  const url = await resolveUrl(page, DASHBOARD_CANDIDATES, process.env.DASHBOARD_URL);
  if (!url) test.skip(true, 'No se encontró Aspire Dashboard en puertos conocidos');
  await page.screenshot({ path: `${SCREENSHOTS_DIR}/aspire-dashboard.png`, fullPage: true });
});

test('Captura Frontend Principal', async ({ page }) => {
  ensureDir(SCREENSHOTS_DIR);
  const url = await resolveUrl(page, FRONTEND_CANDIDATES, process.env.FRONTEND_URL);
  if (!url) test.skip(true, 'No se encontró Frontend en puertos conocidos');
  await page.screenshot({ path: `${SCREENSHOTS_DIR}/frontend-main.png`, fullPage: true });
});
