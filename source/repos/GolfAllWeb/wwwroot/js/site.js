// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Buscador inteligente para ProductosGolf
//$(document).ready(function () {
//    var $input = $("input[name='SearchTerm']");
//    var $cards = $(".producto-card").closest('.col-md-4');

//    $input.on('input', function () {
//        var term = $input.val().toLowerCase();
//        $cards.each(function () {
//            var $card = $(this);
//            var text = $card.text().toLowerCase();
//            if (term === "" || text.indexOf(term) !== -1) {
//                $card.show();
//            } else {
//                $card.hide();
//            }
//        });
//    });

$(function () {
    // Cambia eso al puerto de tu API
    const apiBaseUrl = "https://localhost:7027";
    // Cargar tipos de productos al cargar la página
    $(document).on("click", ".tipo-card", function () {
        var tipo = $(this).data("tipo");
        var url = tipo
            ? `${apiBaseUrl}/ProductosGolf/catalogo/tipo/${encodeURIComponent(tipo)}`
            : `${apiBaseUrl}/ProductosGolf/catalogo`;

        $.get(url, function (data) {
            var $lista = $("#productos-lista");
            $lista.empty();
            if (!data || data.length === 0) {
                $lista.append('<div class="col-12 text-center"><p class="lead">No hay productos disponibles.</p></div>');
            } else {
                data.forEach(function (producto) {
                    $lista.append(`
                        <div class="col-md-4 mb-4">
                        <h4>${tipo}</h4>
                            <div class="card producto-card h-100">
                                <img src="${producto.imagenUrl}" class="card-img-top" alt="${producto.nombre}" style="height: 200px; object-fit: cover; border-radius: 16px 16px 0 0;">
                                <div class="card-body">
                                    <h5 class="card-title producto-title">${producto.nombre}</h5>
                                    <p class="card-text producto-tipo">Tipo: ${producto.tipo}</p>
                                    <p class="card-text producto-marca">Marca: ${producto.marca}</p>
                                </div>
                            </div>
                        </div>
                    `);
                });
            }
        });
    });
});
