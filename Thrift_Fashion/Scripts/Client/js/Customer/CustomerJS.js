var customer = {
    init: function () {
        customer.registerEvents();
    },
    registerEvents: function () {
        console.log("hjdshj");
        $('#txtUserName').focusout(function () {
            $.ajax({
                url: '/Customers/FillPassword',
                data: { userName: $('#txtUserName').val() },
                type: 'POST',
                dataType: 'json',
                success: function (res) {
                    if (res.Status == true) {
                        $('#txtPassword').val(res.Data);
                    }
                }
            })
        });

        /*lấy phần tử modal */
        var modal = document.getElementById("modalForgotPassword");

        /* thiết lập nút đóng modal */
        var span = document.getElementsByClassName("close")[0];

        /* Sẽ hiển thị modal khi người dùng click vào */
        $("#openModal").on("click", function () {
            modal.style.display = "block";
        })

        /* Sẽ đóng modal khi nhấn dấu x */
        $("#btnClose").on("click",function () {
            modal.style.display = "none";
        })

        /*Sẽ đóng modal khi nhấp ra ngoài màn hình*/
        window.onclick = function (event) {
            if (event.target == modal) {
                modal.style.display = "none";
            }
        }

        $("#btnSubmit").on("click", function (e) {
            console.log("hs");
            e.preventDefault();
            $.ajax({
                url: "/Customers/ForgotPassword",
                data: { email: $("#txtEmail").val() },
                dataType: "json",
                type: "POST",
                success: function (res) {
                    if (res.Status == true) {
                        window.location.href = "/Customers/UpdatePassword";
                    }
                    else {
                        window.location.href = "/Customers/CustomerLogin";
                    }
                }
            })
        });
    }
}

customer.init();
