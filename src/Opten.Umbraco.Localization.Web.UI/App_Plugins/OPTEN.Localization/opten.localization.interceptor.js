; (function () {
	'use strict';

	function applyTemplateModification(response, insertString, config) {
		for (var i = 0; i < config.length; i++) {
			if (typeof config[i].url === 'string') {
				config[i].url = [config[i].url];
			}
			for (var j = 0; j < config[i].url.length; j++) {
				if (response.config.url.toLowerCase().indexOf(config[i].url[j].toLowerCase()) >= 0) {
					var search = config[i].insertAfter.toLowerCase();

					var indexOfSearch = response.data.toLowerCase().indexOf(search);

					if (indexOfSearch >= 0) {
						var position = indexOfSearch + search.length;

						response.data = [response.data.slice(0, position), insertString, response.data.slice(position)].join('');
					}
				}
			}
		}
	}

	app.factory('localizationHttpResponseInterceptor', ['$q', '$injector', function ($q, $injector) {
		return {
			response: function (response) {

				if (response.config.url.toLowerCase().indexOf("opten.localization.config.json") >= 0) {
					return response;
				}

				var deffered = $q.defer();

				var $http = $injector.get('$http');
				$http.get('/config/opten.localization.config.json', { cache: true })
					.then(function (json) {

						json.data.forEach(function (element, index) {

							applyTemplateModification(response, element.insertString, element.modifications);

						});

						deffered.resolve(response);
					});

				return deffered.promise;
			}
		}
	}]);

	app.config(['$httpProvider', function ($httpProvider) {
		$httpProvider.interceptors.push('localizationHttpResponseInterceptor');
	}]);

}());