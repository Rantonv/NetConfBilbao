// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Buscador inteligente para ProductosGolf
$(document).ready(function () {
    var $input = $("input[name='SearchTerm']");
    var $cards = $(".producto-card").closest('.col-md-4');

    $input.on('input', function () {
        var term = $input.val().toLowerCase();
        $cards.each(function () {
            var $card = $(this);
            var text = $card.text().toLowerCase();
            if (term === "" || text.indexOf(term) !== -1) {
                $card.show();
            } else {
                $card.hide();
            }
        });
    });
});
