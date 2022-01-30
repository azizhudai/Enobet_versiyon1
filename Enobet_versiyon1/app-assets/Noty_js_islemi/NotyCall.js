if ($.noty) {
    var n_dom = [];
    n_dom[0] = '<div class="activity-item"> <i class="zmdi zmdi-check-all"></i> <div class="activity"> There are <a href="#">6 new tasks</a> waiting for you. Don\'t forget! <span>About 3 hours ago</span> </div> </div>',

    n_dom[1] = '<div class="activity-item"> <i class="zmdi zmdi-alert-polygon"></i> <div class="activity"> Mail server was updated. See <a href="#">changelog</a> <span>About 2 hours ago</span> </div> </div>',

    n_dom[2] = '<div class="activity-item"> <i class="zmdi zmdi-email"></i> <div class="activity"> Your <a href="#">latest post</a> was liked by <a href="#">Audrey Mall</a> <span>35 minutes ago</span> </div> </div>',

    n_dom[3] = '<div class="activity-item"> <i class="zmdi zmdi-shopping-cart-plus"></i> <div class="activity"> <a href="#">Eugene</a> ordered 2 copies of <a href="#">OEM license</a> <span>14 minutes ago</span> </div> </div>',

    n_dom[4] = '<div class="activity-item"> <i class="zmdi zmdi-truck"></i> <div class="activity"> <a href="#">Amark</a> This is frienly notification example <a href="#">Here</a> <span>14 minutes ago</span> </div> </div>',

    n_dom[5] = '<div class="activity-item"> <i class="zmdi zmdi-favorite-outline"></i> <div class="activity"> <a href="#">Amark</a> This is frienly notification example <a href="#">Here</a> <span>14 minutes ago</span> </div> </div>';

    window.anim = {};
    window.anim.open = 'flipInX';
    window.anim.close = 'flipOutX';
    $('#anim-open').on('change', function (e) {
        window.anim.open = $(this).val();
    });


    $('#anim-close').on('change', function (e) {
        window.anim.close = $(this).val();
    });


    var nGen = function nGen(type, text, layout) {
        var n = noty({
            text: text,
            type: type,
            dismissQueue: true,
            layout: layout,
            closeWith: ['click'],
            theme: 'ThemeNoty',
            maxVisible: 10,
            animation: {
                open: 'noty_animated bounceInRight',
                close: 'noty_animated bounceOutRight',
                easing: 'swing',
                speed: 500
            }
        });
        setTimeout(function () {
            n.close();
        }, 5000);
    }

    var nGenAll = function nGenAll() {
        nGen('warning', n_dom[0], 'topRight');
        nGen('error', n_dom[1], 'topRight');
        nGen('information', n_dom[2], 'topRight');
        nGen('success', n_dom[3], 'topRight');
        nGen('alert', n_dom[4], 'topRight');
    }

    setTimeout(function () {
        nGenAll();
    }, 3000);

    var PreviewGen = function PreviewGen(type, text, layout) {
        var n = noty({
            text: text,
            type: type,
            dismissQueue: true,
            layout: layout,
            closeWith: ['click'],
            theme: 'ThemeNoty',
            maxVisible: 10,
            animation: {
                open: 'noty_animated bounceInDown',
                close: 'noty_animated fadeOut',
                easing: 'swing',
                speed: 500
            }
        });
        setTimeout(function () {
            n.close();
        }, 5000);
    }

    $('.ex-noty').on('click', function () {
        var Dtype = $(this).data("type"),
            Dlayout = $(this).data("layout");
        PreviewGen(Dtype, n_dom[2], Dlayout);
    });
}