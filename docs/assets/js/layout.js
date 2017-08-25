$(document).ready(function () {
    var AFFIX_TOP_LIMIT = 300;
    var AFFIX_OFFSET = 49;
	//$('#menu-left').localScroll({hash:true, onAfterFirst:function(){$('html, body').scrollTo( {top:'-=25px'}, 'fast' );}});
    var $menu = $("#menu"),
		$btn = $("#menu-toggle");

    $("#menu-toggle").on("click", function () {
        $menu.toggleClass("open");
        return false;
    });

    $(".docs-nav").each(function () {
        var $affixNav = $(this),
			$container = $affixNav.parent(),
			affixNavfixed = false,
			originalClassName = this.className,
			current = null,
			$links = $affixNav.find("a");


        $(window).on("scroll", function (evt) {
            var top = window.scrollY,
		    	height = $affixNav.outerHeight(),
		    	max_bottom = $container.offset().top + $container.outerHeight(),
		    	bottom = top + height + AFFIX_OFFSET;

            if (affixNavfixed) {
                if (top <= AFFIX_TOP_LIMIT) {
                    $affixNav.removeClass("fixed");
                    $affixNav.css("top", 0);
                    affixNavfixed = false;
                } else if (bottom > max_bottom) {
                    $affixNav.css("top", (max_bottom - height) - top);
                } else {
                    $affixNav.css("top", AFFIX_OFFSET);
                }
            } else if (top > AFFIX_TOP_LIMIT) {
                $affixNav.addClass("fixed");
                affixNavfixed = true;
            }
        });
    });

    // set external links to open in new window/tab
    $(document.links).filter(function() {
        return this.hostname != window.location.hostname;
    }).attr('target', '_blank');
});
