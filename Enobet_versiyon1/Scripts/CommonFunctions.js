
$.ajax({
    type: "POST",
    //url: "@Url.Action("PersonelMazeretListTable", "IstatistikIslemleri")",
    data: JSON.stringify({}),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    error: function (ex) {
        $("#messageBoxModal .modal-title").html("Nöbet Mazeret Liste İşlemi");
        //$("#messageBoxModal .message-image").children().eq(0).attr("src", "/Content/img/Warning.png");
        $("#messageBoxModal .modal-body").html("<p>Hata Oluştu</p>");
        $('#messageBoxModal').modal();
    },
    success: function (data) {

        if (data.State == 1) {

            if ($.fn.DataTable.isDataTable('#PersonelMazeretListTable')) {
                $('#PersonelMazeretListTable').DataTable().clear().draw();
            }

            // debugger;
            ////////////
            var fileName = "Foça Bilgi Sistem Amirliği E-Nöbet Personel Mazeret Listesi";
            var title = 'Personel Mazeret Listesi';
            if (data.NobetMazeretList == null)
                return;
            if (data.NobetMazeretList.length <= 0)
                return;

            var NobetList = JSON.parse(data.NobetMazeretList);  //JSON.stringify(data.NobetList)
            var NobetListTable = $('#PersonelMazeretListTable').DataTable({
                "data": NobetList,
                "columns": [
                    { title: "Mahal Adı", data: "MahalAdi" },
                    { title: "Personel Adı", data: "TalepEdenFulAdi" },
                    { title: "Başlangıç Tarihi", data: "NobetTarihi" },
                    { title: "Bitiş Tarihi", data: "NobetTarihi" },
                    { title: "Mazeret Açıklama", data: "NobetTarihi" },
                    { title: "İşlem", data: "NobetTarihi" }
                ],
                columnDefs: [
                    {
                        "targets": 2,
                        "data": "NobetTarihi",
                        "render": function (data, type, row, meta) {
                            var dateArray = data.split('-');
                            var date = DateFormatTR(dateArray[0], (dateArray[1] - 1), dateArray[2]);
                            return date;
                        }
                    }
                ],
                "ordering": false,
                paging: false,
                dom: 'Bfrtip',
                buttons: [
                    'print',
                    { extend: 'pdf', className: 'pdfButton', title: title, filename: fileName, charset: 'UTF-8', },
                    {
                        extend: 'csv',
                        charset: 'UTF-8',
                        fieldSeparator: ';',
                        bom: true,
                        className: 'excelButton',
                        filename: fileName,
                        title: title
                    }
                ]
            });


            $(".excelButton").text("CSV İndir");
            $(".buttons-print").text("Çıktı Al");
            $(".pdfButton").text("PDF İndir");



        }

    }


});