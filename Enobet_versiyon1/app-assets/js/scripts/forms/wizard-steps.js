/*=========================================================================================
    File Name: wizard-steps.js
    Description: wizard steps page specific js
    ----------------------------------------------------------------------------------------
    Item Name: Frest HTML Admin Template
    Version: 1.0
    Author: PIXINVENT
    Author URL: http://www.themeforest.net/user/pixinvent
==========================================================================================*/
//    Wizard tabs with icons setup
// ------------------------------
var horizontal = $(".wizard-horizontal");
var form2 = horizontal.show();
horizontal.steps({
    headerTag: "h6",
    bodyTag: "fieldset",
    transitionEffect: "fade",
    titleTemplate: '<span class="step">#index#</span> #title#',
    labels: {
        finish: 'Talep Et'
    },
    onStepChanging: function (event, currentIndex, newIndex) {
        //debugger;
        // Allways allow previous action even if the current form is not valid!
        if (currentIndex > newIndex) {
            //alert("denem");
            return true;
        }
        form2.validate().settings.ignore = ":disabled,:hidden";
        return form2.valid();
    },
    onFinishing: function (event, currentIndex) {
        form2.validate().settings.ignore = ":disabled";
        return form2.valid();
    },
    onFinished: function (event, currentIndex) {
        //alert("Form submitted.   " + event + '   ' + currentIndex);
        var type = 0;

        var data = [];
        data[0] = $('#NobetListeId').val();
        
        if (currentIndex == 2) {
            //karşılıklı nöbet değiş
            if ($('#searchInputOnayMak').val() == "") {
                alert("Onay Makamı Zorunlu Alan!");
                return;
            }
            type = 2;
            data[1] = $('#OnEskiNobetTarihiKendisi').data('tarih');//  $('#OnYeniNobetTarihiKendisi').val(); //diger eski nt
            data[2] = $('#OnYeniNobetTarihiKendisi').data('tarih'); //$('#OnEskiNobetTarihiKendisi').val(); // diğer yenisi nt


            data[3] = type;


            data[4] = $('#NobetTarihiKendisi').val();// kendi uid
            data[5] = $('#NobetTarihiBaskasi').val();// talep ettigi uid

            data[6] = $('#OnEskiNobetTarihiKendisi').data('tarih'); //$('#OnYeniNobetTarihiKendisi').val(); // kendi yeni nt
            data[7] = $('#OnYeniNobetTarihiKendisi').data('tarih'); // $('#OnEskiNobetTarihiKendisi').val(); // kendi eski nt

            data[8] = $('#MahalId').val();

            data[9] = $('#uIdKendiAmir').val();
            data[10] = $('#uIdTalepAmir').val();
            data[11] = $('#uIdOnayMak').val();

            var arryKendi = $('#NobetTarihiKendisi option:selected').text().split('||');
            data[12] = arryKendi[1];

            var arryBaska = $('#NobetTarihiBaskasi option:selected').text().split('||');
            data[13] = arryBaska[1];

            // data[12] = $('#AmirEposta').val();

            data[14] = $('#Mazeret').val();

            data[15] = $('#NobetKidemliId').val();
            /*var arryEskiKendiNT = $('#NobetTarihiKendisi').val();
            data[5] = arryEskiKendiNT[0];
            var arryYeniKendiNT = $('#NobetTarihiKendisi').val();*/


            // data[7] = $('#NobetTarihiBaskasi').val();
            //değişim tipi 2 olcak

            AjaxMethod(data);

        }
        //else if (currentIndex == 1) {
        //    //tek gün nöbet değiş
        //    type = 1;
        //    //data[1] = null;
        //  //  data[2] = null;
        //    data[1] = type;
         
        //    data[2] = $('#TekNobetTarihiKendisi option:selected').val();
        //    data[3] = $('#MahalId').val();
        //    data[4] = $('#uIdTekTalepKendiAmir').val();
            
        //    data[5] = $('#uIdTekTalepEttigiAmir').val();
        //    data[6] = $('#uIdTekOnayMak').val();
        //    data[7] = $('#TekOnizlemeAciklama').val();

        //    data[8] = $('#NobetKidemliId').val();

        //    data[9] = $('#TekOnYeniNobetTarihiKendisi').val();
        //    data[10] = $('#TekTalepEttigiPersonelId').val();
        //    AjaxMethodTekTalep(data);
        //}
       /* else if (currentIndex == 3) {*/

           

      /*  }*/
      
    }
});

//////
var horizontal22 = $(".wizard22-horizontal");
var form22 = horizontal22.show();
horizontal22.steps({
    headerTag: "h6",
    bodyTag: "fieldset",
    transitionEffect: "fade",
    titleTemplate: '<span class="step">#index#</span> #title#',
    labels: {
        finish: 'Talep Et'
    },
    onStepChanging: function (event, currentIndex, newIndex) {
         if (currentIndex == 0) {
            var TekNobetTarihiKendisi = $("#TekNobetTarihiKendisi option:selected").text().split('||');
            $("#TekOnYeniNobetTarihiKendisi").val(TekNobetTarihiKendisi[0]);

            var searchInputTekTalepKendiAmir = $('#searchInputTekTalepKendiAmir').val();
            $('#TekAmirAdi').val(searchInputTekTalepKendiAmir);

            var searchInputTekTalepEttigiAmir = $('#searchInputTekTalepEttigiAmir').val();
            $('#TekTalepEdeninAmirAdi').val(searchInputTekTalepEttigiAmir);

            //$('#TekOnizlemeAciklama').text();

            //var textArry33 = $("#NobetTarihiKendisi option:selected").text().split('||');
            var aciklama = $('#IslemYapanAdi').val() + " PERSONELİN NÖBET TARİHİ " + TekNobetTarihiKendisi[0] + " OLACAKTIR.";

            $('#TekOnizlemeAciklama').text(aciklama);

        }
       // debugger;
        // Allways allow previous action even if the current form is not valid!
        if (currentIndex > newIndex) {
            //alert("denem");
            return true;
        }
        form22.validate().settings.ignore = ":disabled,:hidden";
        return form22.valid();
    },
    onFinishing: function (event, currentIndex) {
        form22.validate().settings.ignore = ":disabled";
        return form22.valid();
    },
    onFinished: function (event, currentIndex) {
        //alert("Form submitted.   " + event + '   ' + currentIndex);
        //debugger;
        

        var type = 0;

        var data = [];
        data[0] = $('#NobetListeId').val();
         if (currentIndex == 1) {
            //tek gün nöbet değiş
            type = 1;
            //data[1] = null;
            //  data[2] = null;
            data[1] = type;

            data[2] = $('#TekNobetTarihiKendisi option:selected').val();
            data[3] = $('#MahalId').val();
            data[4] = $('#uIdTekTalepKendiAmir').val();

            data[5] = $('#uIdTekTalepEttigiAmir').val();
            data[6] = $('#uIdTekOnayMak').val();
            data[7] = $('#TekOnizlemeAciklama').text();

            data[8] = $('#NobetKidemliId').val();

             data[9] = $('#TekOnYeniNobetTarihiKendisi').val();
             data[10] = $('#TekMazeret').val();
            //data[10] = $('#TekTalepEttigiPersonelId').val();
             if (data[5] == "" || data[5] == null || data[5] == "0") {
                 alert("Talep Eden Personelin Amirini Seçiniz!");
                return;
             }
                 
             if (data[6] == "" || data[6] == null || data[6] == "0") {
                 alert("Onay Makamı Seçiniz!");
                 return;
             }
            AjaxMethodTekTalep(data);
        }
        /* else if (currentIndex == 3) {*/



        /*  }*/

    }
});



// Initialize validation
horizontal.validate({
    ignore: 'input[type=hidden]', // ignore hidden fields
    errorClass: 'danger',
    successClass: 'success',
    highlight: function (element, errorClass) {
        $(element).removeClass(errorClass);
    },
    unhighlight: function (element, errorClass) {
        $(element).removeClass(errorClass);
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    rules: {
        email: {
            email: true
        }
    }
});

// Initialize validation
horizontal22.validate({
    ignore: 'input[type=hidden]', // ignore hidden fields
    errorClass: 'danger',
    successClass: 'success',
    highlight: function (element, errorClass) {
        $(element).removeClass(errorClass);
    },
    unhighlight: function (element, errorClass) {
        $(element).removeClass(errorClass);
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    rules: {
        email: {
            email: true
        }
    }
});

//        vertical Wizard       //
// ------------------------------
$(".wizard-vertical").steps({
    headerTag: "h3",
    bodyTag: "fieldset",
    transitionEffect: "fade",
    enableAllSteps: true,
    stepsOrientation: "vertical",
    labels: {
        finish: 'Submit'
    },
    onFinished: function (event, currentIndex) {
        alert("Form submitted.   " + event + '   ' + currentIndex);

    }
});


//       Validate steps wizard //
// -----------------------------
// Show form
var stepsValidation = $(".wizard-validation");
var form = stepsValidation.show();

stepsValidation.steps({
  headerTag: "h6",
  bodyTag: "fieldset",
  transitionEffect: "fade",
  titleTemplate: '<span class="step">#index#</span> #title#',
  labels: {
    finish: 'Submit'
  },
  onStepChanging: function (event, currentIndex, newIndex) {
    // Allways allow previous action even if the current form is not valid!
      if (currentIndex > newIndex) {
        alert("denem");
      return true;
    }
    form.validate().settings.ignore = ":disabled,:hidden";
    return form.valid();
  },
  onFinishing: function (event, currentIndex) {
    form.validate().settings.ignore = ":disabled";
    return form.valid();
  },
  onFinished: function (event, currentIndex) {
    alert("Submitted!");
  }
});

// Initialize validation
stepsValidation.validate({
  ignore: 'input[type=hidden]', // ignore hidden fields
  errorClass: 'danger',
  successClass: 'success',
  highlight: function (element, errorClass) {
    $(element).removeClass(errorClass);
  },
  unhighlight: function (element, errorClass) {
    $(element).removeClass(errorClass);
  },
  errorPlacement: function (error, element) {
    error.insertAfter(element);
  },
  rules: {
    email: {
      email: true
    }
  }
});
// live Icon color change on state change
$(document).ready(function () {
  $(".current").find(".step-icon").addClass("bx bx-time-five");
  $(".current").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
    strokeColor: '#5A8DEE'
  });
});
// Icon change on state
// if click on next button icon change
$(".actions [href='#next']").click(function () {
  $(".done").find(".step-icon").removeClass("bx bx-time-five").addClass("bx bx-check-circle");
  $(".current").find(".step-icon").removeClass("bx bx-check-circle").addClass("bx bx-time-five");
  // live icon color change on next button's on click
  $(".current").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
    strokeColor: '#5A8DEE'
  });
  $(".current").prev("li").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
    strokeColor: '#39DA8A'
  });
});
$(".actions [href='#previous']").click(function () {
  // live icon color change on next button's on click
  $(".current").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
    strokeColor: '#5A8DEE'
  });
  $(".current").next("li").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
    strokeColor: '#adb5bd'
  });
});
// if click on  submit   button icon change
$(".actions [href='#finish']").click(function () {
  $(".done").find(".step-icon").removeClass("bx-time-five").addClass("bx bx-check-circle");
  $(".last.current.done").find(".fonticon-wrap .livicon-evo").updateLiviconEvo({
    strokeColor: '#39DA8A'
  });
});
// add primary btn class
$('.actions a[role="menuitem"]').addClass("btn btn-primary");
$('.icon-tab [role="menuitem"]').addClass("glow ");
$('.wizard-vertical [role="menuitem"]').removeClass("btn-primary").addClass("btn-light-primary");

