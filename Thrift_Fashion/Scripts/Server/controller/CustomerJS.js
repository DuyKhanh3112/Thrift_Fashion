var customer = {
    init: function () {
        customer.registerEvents();
    },
    registerEvents: function () {
       
        $('.btnStatus').on('click', function (e) {
            console.log("hh");
            e.preventDefault();
            console.log("ghs");
            var btn = $(this);
            var id = btn.data('id');

            $.ajax({
                url: "/Customers/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    if (response.Status == true) {
                        btn.text('Enable');
                    }
                    else {
                        btn.text('Disable');
                    }
                }
            });
        });

    }

}
customer.init();