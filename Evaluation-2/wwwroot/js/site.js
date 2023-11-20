$(document).ready(function () {
    
    $("#errorContainer").hide();
    //var groupColumn = 6;
    //var domValue = 'Bfrtip';

    //$("#vehiclesTable").DataTable({
    //    "lengthMenu": [[10, 25, 50, -1], ["Show 10 Rows", "Show 25 Rows", "Show 50 Rows", "Show All Rows"]],
    //    keys: true,
    //    fixedHeader: true,
    //    responsive: true,
    //    select: false,
    //    columnDefs: [
    //        { orderable: false, targets: [0, 1, 2, 3, 4, 5, 6, 7, 8] }
    //    ],
    //    "order": [[0, "desc"]],
    //    language: {
    //        searchBuilder: {
    //            button: '<i class="fa fa-search"></i>',
    //        }
    //    },
    //    dom: domValue,
    //    buttons: [

    //        {
    //            extend: 'pageLength'
    //        },
    //        {
    //            extend: 'searchBuilder',
    //            config: {
    //                columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
    //            }
    //        },
    //        {
    //            extend: 'collection',
    //            text: '<i class="fa fa-cog"></i>',
    //            buttons: ['csv', 'excel', 'pdf']
    //        },
    //        {
    //            extend: 'print',
    //            text: '<i class="fa fa-print"></i>',
    //            exportOptions: {
    //                columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
    //            }
    //        }, {
    //            extend: 'colvis',
    //            text: '<i class="fa fa-columns"></i>',

    //        },

    //        {
    //            text: '<i class="fa fa-refresh"></i>',
    //            action: function (e, dt, node, config) {
    //                $('#viewAll').load('?handler=ViewAllPartial');
    //            }
    //        },
    //    ],
    //    destroy: true,
    //    //statesave: false,

    //});

    $('#viewAll').load('?handler=ViewAllPartial');

    $(function () {
        $('#reload').on('click', function () {
            $('#viewAll').load('?handler=ViewAllPartial');
        });
    });



    $("#createVehicle").validate({
        rules: {

            RegNo: { required: true, maxlength: 50 },
            Make: { required: true, maxlength: 50 },
            Model: { required: true, maxlength: 50 },
            Color: { required: true, maxlength: 50 },
            EngineNo: { required: true, maxlength: 50 },
            ChasisNo: { required: true, maxlength: 50 },
            DateOfPurchase: { required: true },


        },
        messages: {
            
            RegNo: { required: "Registration No is reqiured", maxlength: "Registration No cannot be longer than 50 chars" },
            Make: { required: "Make is reqiured", maxlength: "Make cannot be longer than 50 chars" },
            Model: { required: "Model is reqiured", maxlength: "Model cannot be longer than 50 chars" },
            Color: { required: "Color is reqiured", maxlength: "Color cannot be longer than 50 chars" },
            EngineNo: { required: "Engine No is reqiured", maxlength: "Engine No cannot be longer than 50 chars" },
            ChasisNo: { required: "Chasis No is reqiured", maxlength: "Chasis No cannot be longer than 50 chars" },
            DateOfPurchase: { required: "Date of Purchase is reqiured" }
        },
        highlight: function (element, errorClass, validClass) {
            $("#errorContainer").fadeIn();
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass('is-invalid');
        },
        errorElement: 'li',
        errorLabelContainer: '#errorContainer ul',
        submitHandler: function (form) {
            var formdata = new FormData(form);
            alert("");
             try {
     
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                beforeSend: function () {
                    $("#errorContainer").fadeOut();
                },
                success: function (response) {
                    if (res.isValid) {
                        $('#viewAll').html(res.html)
                    }
                },
                failure: function (response) {
                    showToast("Sorry, There was some error,please try again", TOAST_STATUS.DANGER);
                },
                error: function (response) {
                    showToast("Sorry, There was some error,please try again", TOAST_STATUS.DANGER);
                }
            });
             return false;
        } catch (ex) {
            console.log(ex)
        }
           

        }
    });
    
    $(function () {
        $('#reset').on('click', function () {
            ResetForm();
        });
    });

});


function ResetForm() {

    $("#RegNo").val("");
    $("#Make").val("");
    $("#Model").val("");
    $("#Color").val("");
    $("#EngineNo").val(1);
    $("#ChasisNo").val("");
    $("#DateOfPurchase").val("");
    $("#Active").val("");
    $("#errorContainer").hide();
    $(".form-control").removeClass("is-invalid");
}


function SubmitData() {
 try {
     
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                beforeSend: function () {
                    $("#errorContainer").fadeOut();
                },
                success: function (response) {
                    if (res.isValid) {
                        $('#viewAll').html(res.html)
                    }
                },
                failure: function (response) {
                    showToast("Sorry, There was some error,please try again", TOAST_STATUS.DANGER);
                },
                error: function (response) {
                    showToast("Sorry, There was some error,please try again", TOAST_STATUS.DANGER);
                }
            });
             return false;
        } catch (ex) {
            console.log(ex)
        }
}