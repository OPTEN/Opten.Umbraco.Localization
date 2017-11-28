; (function () {
	'use strict';

	app.run(initializeLocalization);

	initializeLocalization.$inject = ["$rootScope", "eventsService", "languageResource"];

	function initializeLocalization($rootScope, eventsService, languageResource) {
		eventsService.on("app.authenticated", function (e, d) {
			languageResource.getAllLanguages().then(function (data) {

				// Add all languages to the root scope
				$rootScope.isLoading = true; // Hack: because selectedLanguages = [] will call the controller with wrong values...

				$rootScope.languages = [];
				$rootScope.selectedLanguages = [];
				$rootScope.defaultLanguages = [];
				angular.forEach(data, function (value, key) {
					var language = {
						isoCode: value.isoCode,
						twoLetterISOCode: value.isoCode.split('-')[0],
						name: value.name,
						select: function () {
							if ($rootScope.selectedLanguages.indexOf(this.isoCode) == -1) {
								$rootScope.selectedLanguages.push(this.isoCode);
							} else {
								$rootScope.selectedLanguages.splice($rootScope.selectedLanguages.indexOf(this.isoCode), 1);
							}
						}
					};

					if (value.isDefault) {
						$rootScope.defaultLanguages.push(language.isoCode);
					}

					$rootScope.languages.push(language);
				});

				setSelectedLanguages();
			});

			// Handle language change

			$rootScope.$watch("selectedLanguages", function (value) {
				// we have to have some languages even if it is empty array.
				if (value) {
					storeSelectedLanguages(value);

					if ($rootScope.isLoading) {
						$rootScope.isLoading = false;
					}
				}
			}, true);

			$rootScope.selectAllLanguages = function () {
				$rootScope.selectedLanguages = $rootScope.languages.map(function (o) { return o.isoCode; });
			};

			$rootScope.deselectAllLanguages = function () {
				$rootScope.selectedLanguages = [];
			};


			// Private functions

			function setSelectedLanguages() {
				// Set stored language as current

				languageResource.getSelectedLanguages().then(function (languages) {
					if (languages && languages.length) {
							$rootScope.selectedLanguages = languages.map(function (l) {
								return l.isoCode;
							});
					}
				});
			};

			function storeSelectedLanguages(isoCodes) {
				if (!$rootScope.isLoading && isoCodes) {
					// post and store the language in a cookie
					languageResource.postSelectedLanguages(isoCodes);
				}
			};
		});
	};

}());