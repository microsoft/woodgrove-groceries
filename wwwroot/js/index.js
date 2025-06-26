function stepUpProductFilter() {
    document.addEventListener('DOMContentLoaded', function () {
        const categoryFilter = document.getElementById('categoryFilter');
        const searchInput = document.getElementById('searchInput');
        const products = document.querySelectorAll('.product-item');

        function filterProducts() {
            const selectedCategory = categoryFilter.value.toLowerCase();
            const searchTerm = searchInput.value.toLowerCase();

            products.forEach(product => {
                const productCategory = product.dataset.category.toLowerCase();
                const productName = product.dataset.name;
                const matchesCategory = !selectedCategory || productCategory === selectedCategory;
                const matchesSearch = !searchTerm || productName.includes(searchTerm);

                if (matchesCategory && matchesSearch) {
                    product.style.display = '';
                } else {
                    product.style.display = 'none';
                }
            });
        }

        categoryFilter.addEventListener('change', filterProducts);
        searchInput.addEventListener('input', filterProducts);
    });
}


stepUpProductFilter();

var thanksModal, stepUpModal;
$(document).ready(function () {
    $('.card-allergy').popover({ trigger: "hover", content: "Please be aware that the food may contain or come into contact with eggs." });

    thanksModal = new bootstrap.Modal(document.getElementById('thanksModal'), {
        keyboard: false
    });

    stepUpModal = new bootstrap.Modal(document.getElementById('stepUpModal'), {
        keyboard: false
    });
});

function addItem(price) {
    var total = parseInt($("#total").text(), 0) + price;
    $("#total").text(total);
    $("#checkoutContainer").show();

    return false;
}

function checkout() {
    var total = parseInt($("#total").text(), 0);

    if (total > 50 && $("#stepUpFulfilled").text() != "True") {
        stepUpModal.show();
    }
    else {
        completeOrder();
    }

    return false;
}

function completeOrder() {
    $("#total").text(0);
    $("#checkoutContainer").hide();

    thanksModal.show();
    window.location.hash = '';
    const myTimeout = setTimeout(completeOrderHide, 5000);
}

function completeOrderHide() {
    thanksModal.hide();
}
