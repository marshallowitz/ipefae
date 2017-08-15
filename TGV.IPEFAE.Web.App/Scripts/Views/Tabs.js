var originalHeight = 0;

jQuery(document).ready(function ($)
{
	var tabItems = $('.cd-tabs-navigation a'),
		tabContentWrapper = $('.cd-tabs-content');

	tabItems.on('click', function (event)
	{
	    event.preventDefault();

	    if ($(this).hasClass("disabled"))
	        return false;

	    var selectedItem = $(this);
	    if (!selectedItem.hasClass('selected'))
	    {
	        var selectedTab = selectedItem.data('content'),
				selectedContent = tabContentWrapper.find('li[data-content="' + selectedTab + '"]'),
				slectedContentHeight = selectedContent.innerHeight();

	        tabItems.removeClass('selected');
	        selectedItem.addClass('selected');
	        selectedItem.parent().find('span').addClass('selected');
	        selectedItem.parent().siblings('li').find('span').removeClass('selected');
	        selectedContent.addClass('selected').siblings('li').removeClass('selected');
	        //animate tabContentWrapper height when content changes 
	        //tabContentWrapper.animate({ 'height': slectedContentHeight }, 200);
	    }

	    tabsContentResize(65, true);
	    setTabsContentHeight(function ()
	    {
	        $('.provas_gabaritos').height($('.provas_gabaritos').find('.col-xs-6').height());
	        
	        if (selectedItem.attr('data-content') == "provas_gabaritos")
	        {
	            setTimeout(function () { $('.provas_gabaritos').parent().height($('.provas_gabaritos').find('.col-xs-6').height() + 80); }, 500);
	        }
	        else if (selectedItem.attr('data-content') == "publicacoes")
	        {
	            tabsContentResize(65, true);
	        }

	    }, $('.provas_gabaritos').find('.col-xs-6 > table > tbody'), 0);
	});

	//hide the .cd-tabs::after element when tabbed navigation has scrolled to the end (mobile version)
	checkScrolling($('.cd-tabs nav'));
	$(window).on('resize', function(){
		checkScrolling($('.cd-tabs nav'));
		tabContentWrapper.css('height', 'auto');
	});
	$('.cd-tabs nav').on('scroll', function(){ 
		checkScrolling($(this));
	});

	function checkScrolling(tabs){
		var totalTabWidth = parseInt(tabs.children('.cd-tabs-navigation').width()),
		 	tabsViewport = parseInt(tabs.width());
		if( tabs.scrollLeft() >= totalTabWidth - tabsViewport) {
			tabs.parent('.cd-tabs').addClass('is-ended');
		} else {
			tabs.parent('.cd-tabs').removeClass('is-ended');
		}
	}
});