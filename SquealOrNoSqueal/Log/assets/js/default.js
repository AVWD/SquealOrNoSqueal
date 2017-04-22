;!(function ($) {
    $.fn.classes = function(callback) {
        var classes = [];
        $.each(this, function(i, v) {
            var splitClassName = v.className.split(/\s+/);
            for (var j in splitClassName) {
                var className = splitClassName[j];
                if (-1 === classes.indexOf(className)) {
                    classes.push(className);
                }
            }
        });
        if ('function' === typeof callback) {
            for (var i in classes) {
                callback(classes[i]);
            }
        }
        return classes;
    };
})(jQuery);

$(function() {
	var btnContainer = $('#channelContainer');

	$('.post').classes(function(c) {
        if(c !== "post") {
			var newBtn = $('<button>');   	
			newBtn.addClass('toggleButton ' + c);
			newBtn.html(c);
			newBtn.click(function(){
				$('div.post.' + $(this).html()).toggle();
				return false;
			});
			
			btnContainer.append(newBtn);
        }
    });

	$('.stackToggle').click(function(){
		$(this).siblings('div.stackMsg').toggle();
		return false;
	});

});