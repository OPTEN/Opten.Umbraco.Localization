; (function () {
	'use strict';

	angular.module("umbraco.directives").directive("optenLanguageShow", languageShowDirective);

	languageShowDirective.$inject = ["$rootScope", "$animator"];

	function languageShowDirective($rootScope, $animator) {
		var directive = {
			restrict: "A",
			link: link
		};

		return directive;


		// Private functions

		function link(scope, element, attr) {
			scope.$watch(attr.optenLanguageShow, function (value) {
				var propertyAlias = value,
					canLocalize = false,
					language = "",
					animate = $animator(scope, attr);

				angular.forEach($rootScope.languages, function (value, key) {
					if (typeof propertyAlias !== "undefined" && propertyAlias.indexOf("_" + value.twoLetterISOCode) > -1) {
						canLocalize = true;
						language = value.isoCode;
					}
				});

				$rootScope.$watch("selectedLanguages", function optenLanguageShowWatchAction(value) {
					animate[(!canLocalize || value.indexOf(language) > -1 || value.length === 0) ? 'show' : 'hide'](element);
				}, true);
			});
		};
	};

	angular.module("umbraco.directives").directive("optenLanguageName", languageNameDirective);

	languageNameDirective.$inject = ["$rootScope", "$animator"];

	function languageNameDirective($rootScope, $animator) {
		var directive = {
			restrict: "A",
			link: link
		};

		return directive;


		// Private functions

		function link(scope, element, attr) {
			scope.$watch(attr.optenLanguageName, function (value) {
				var propertyAlias = value,
					canLocalize = false,
					language = "",
					languageName = "",
					animate = $animator(scope, attr);

				angular.forEach($rootScope.languages, function (value, key) {
					if (typeof propertyAlias !== "undefined" && propertyAlias.indexOf("_" + value.twoLetterISOCode) > -1) {
						canLocalize = true;
						language = value.isoCode;
						languageName = value.nativeName;
					}
				});

				$rootScope.$watch("selectedLanguages", function optenLanguageNameWatchAction(value) {
					animate[(!canLocalize || value.indexOf(language) > -1 || value.length === 0) ? 'show' : 'hide'](element);
					element.text(languageName);
				}, true);
			});
		};
	};

	angular.module("umbraco.directives").directive("optenLanguageMenu", languageMenuDirective);

	function languageMenuDirective() {
		return {
			restrict: "E",
			replace: true,
			templateUrl: "/App_Plugins/OPTEN.Localization/directives/languageMenu.html?v=1.0.0"
		};
	};

}());