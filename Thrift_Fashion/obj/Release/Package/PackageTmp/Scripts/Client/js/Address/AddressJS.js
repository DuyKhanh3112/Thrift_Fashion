var address = {
    init: function () {
        address.registerEvents();
       
    },
    registerEvents: function () {
        $('.btnStatus').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');

            $.ajax({
                url: "/Addresses/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    if (response.status == true) {
                        btn.text('Default');
                    }
                    else {
                        btn.text('Extra');
                    }
                    window.location.href = "/Addresses/Address";
                }
            });
        });

        $('.btnDelete').each(function () {
            $(this).off('click').on('click', function (e) {
                e.preventDefault();
                Swal.fire({
                    title: 'Delete Customer',
                    text: "Are you sure to delete this customer?",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'Cancle',
                    customClass: 'swal-wide'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $.ajax({
                            url: '/Addresses/Delete',
                            data: { id: $(this).data('id') },
                            type: 'POST',
                            dataType: 'json',
                            success: function (res) {
                                if (res.status == true) {
                                    Swal.fire(
                                        'Successfully!',
                                        'Delete successfully this customer',
                                        'success'
                                    );
                                    window.location.href = "/Addresses/Address"
                                }

                            }
                        })

                    }
                })
            })
        })

       
    },
  
   

}
address.init();