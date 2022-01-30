(function ($) {

   $.fn.pickList = function (options) {

       var opts = $.extend({}, $.fn.pickList.defaults, options);

      this.fill = function () {
         var option = '';
          var option2 = '';
          $.each(opts.data, function (key, val) {
             debugger;
             if (val.atamaDurumu === false)
             { option += '<option data-tid=' + val.tid + ' data-id=' + val.id + '>' + val.text + '</option>'; }
             if (val.atamaDurumu === true)
                { option2 += '<option data-tid='+ val.tid +' data-id=' + val.id + '>' + val.text + '</option>';}
         });
         this.find('.pickData').append(option);
          this.find('.pickListResult').append(option2);

      };
      
      this.controll = function () {
         var pickThis = this;

         pickThis.find(".pAdd").on('click', function () {
             debugger;
             var sonuclar = pickThis.getValues();
             var p = pickThis.find(".pickData option:selected");
            // var userAppointId = [];
            // userAppointId = p.data('id');
            // var textUserMail = p.text();
             //var tid = p.data('tid'); // insertte olabilir updatete varm� bak bi burada ��nk� birine g�rev at�yorsun >> silmede aktifi false, 
             var objResult = [];
             var result = "addUser";
             p.each(function () {
                 objResult.push({
                     taskId: $(this).data('tid'),
                     userId: $(this).data('id')

                 });
             });
             var taskIdList = [], userIdList = [];
             for (var i = 0; i < objResult.length;i++) {
                 taskIdList[i] = objResult[i].taskId;
                 userIdList[i] = objResult[i].userId;
             }
             ajaxUsing(taskIdList, userIdList, result);

             p.clone().appendTo(pickThis.find(".pickListResult"));
            
            p.remove();
         });

         pickThis.find(".pAddAll").on('click', function () {
            var p = pickThis.find(".pickData option");
            p.clone().appendTo(pickThis.find(".pickListResult"));
            p.remove();
         });

         pickThis.find(".pRemove").on('click', function () {
             var p = pickThis.find(".pickListResult option:selected");
             var result = "deleteUser";
             var objResult = [];
             p.each(function () {
                 objResult.push({
                     taskId: $(this).data('tid'),
                     userId: $(this).data('id')

                 });
             });
             var taskIdList = [], userIdList = [];
             for (var i = 0; i < objResult.length; i++) {
                 taskIdList[i] = objResult[i].taskId;
                 userIdList[i] = objResult[i].userId;
             }
             ajaxUsing(taskIdList, userIdList, result);
            p.clone().appendTo(pickThis.find(".pickData"));
            p.remove();
         });

         pickThis.find(".pRemoveAll").on('click', function () {
            var p = pickThis.find(".pickListResult option");
            p.clone().appendTo(pickThis.find(".pickData"));
            p.remove();
         });
      };

      this.getValues = function () {
          var objResult = [];
          
         this.find(".pickListResult option").each(function () {
            objResult.push({
               id: $(this).data('id'),
               text: this.text
            });
         });
         return objResult;
      };

      this.init = function () {
         var pickListHtml =
                 "<div class='row'>" +
                 "  <div class='col-sm-5'>Atanmamis Kullanicilar" +
                // " <h1>Atanmamis Kullanicilar</h1>"+
                 "	 <select id='pickListSelect' class='form-control pickListSelect pickData' multiple></select>" +
                 " </div>" +
                
               //  " <div id='form-group' style='width: 100px;'>"+
                     "<td style='width: 10%;'>"+
                 " <div style='height: 150px;' class='col-sm-2 pickListButtons'>" +
                 "	<button type='button' style='width: 70px;' onclick='userAddSelectBtnClick();' class='pAdd btn btn-primary btn-sm'>" + opts.add + "</button>" +
                // "      <button type='button'  class='pAddAll btn btn-red btn-sm'>"+ opts.addAll +  "</button>" +
                 "	<button type='button' style='width: 70px;' class='pRemove btn btn-primary btn-sm'>" + opts.remove + "</button>" +
                // "	<button type='button' class='pRemoveAll btn btn-red btn-sm'>" + opts.removeAll + "</button>" +
                 " </div>" +
                 //" </div>"+
                 "  </td>"+
                 " <div class='col-sm-5'>Atanmis Kullanicilar" +
                // " <h4>Atanmis Kullanicilar</h4>"+
                 "    <select class='form-control pickListSelect pickListResult' id='pickListResultSelect' multiple></select>" +
                 " </div>" +
                 "</div>";

         this.html(pickListHtml);
 
         this.fill();
         this.controll();
      };

      this.init();
      return this;
   };

   $.fn.pickList.defaults = {
      add: 'EKLE -- >',
      remove: '<-- SIL'
    //  addAll: 'Hepsini Ekle',
     // removeAll: 'Hepsini Sil'
   };

   function ajaxUsing(taskIdList, userIdList, result) {
       debugger;
        $.ajax({
            type: "Post",
            url: '/GorevIslemleri/AppointToAddResult',
            data: { taskIdList: taskIdList,userIdList:userIdList, result: result },
            dataType: "json",
            success: function (list) {
                debugger;
                $.notify("Atama Islemi Basarili", "success");
                if (list.tid !== 0 && list.userAppointId !== 0) {

                    
                    // document.getElementById('taskEditModal').reset();
                    //$("#warningModal .modal-body").html("<p>Kay�t Yap�ld�</p>");
                    // $("#warningModal").modal();
                    // $("#taskEditModal").modal("hide");
                }
            }
        });

    }


}(jQuery));


