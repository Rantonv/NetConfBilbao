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
  const errors = [];
  const attempt = async (label, url) => {
    try {
      await page.goto(url, { waitUntil: 'domcontentloaded', timeout: 15000 });
      await page.waitForTimeout(500);
      console.log(`[resolveUrl] OK ${label}: ${url}`);
      return url;
    } catch (e) {
      errors.push({ url, message: e.message });
      console.log(`[resolveUrl] FAIL ${label}: ${url} -> ${e.message}`);
      return null;
    }
  };

  if (preferred) {
    const ok = await attempt('preferred', preferred);
    if (ok) return ok;
  }
  for (const url of candidates) {
    const ok = await attempt('candidate', url);
    if (ok) return ok;
  }
  console.log('[resolveUrl] No reachable URL. Errors:');
  for (const e of errors) console.log(`  - ${e.url}: ${e.message}`);
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
  if (!url) {
    test.skip(true, 'No se encontró Aspire Dashboard en puertos conocidos');
  }
  try {
    await page.screenshot({ path: `${SCREENSHOTS_DIR}/aspire-dashboard.png`, fullPage: true });
  } catch (e) {
    console.log('[dashboard] Screenshot primary attempt failed, retrying minimal wait...', e.message);
    await page.waitForTimeout(1000);
    await page.screenshot({ path: `${SCREENSHOTS_DIR}/aspire-dashboard.png` });
  }
});

test('Captura Frontend Principal', async ({ page }) => {
  ensureDir(SCREENSHOTS_DIR);
  const url = await resolveUrl(page, FRONTEND_CANDIDATES, process.env.FRONTEND_URL);
  if (!url) {
    test.skip(true, 'No se encontró Frontend en puertos conocidos (verifica que el servicio exponga endpoints externos)');
  }
  try {
    await page.screenshot({ path: `${SCREENSHOTS_DIR}/frontend-main.png`, fullPage: true });
  } catch (e) {
    console.log('[frontend] Screenshot primary attempt failed, retrying minimal wait...', e.message);
    await page.waitForTimeout(1000);
    await page.screenshot({ path: `${SCREENSHOTS_DIR}/frontend-main.png` });
  }
});
