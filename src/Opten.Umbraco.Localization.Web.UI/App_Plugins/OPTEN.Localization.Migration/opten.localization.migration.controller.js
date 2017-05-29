; (function () {
	'use strict';

	angular.module("umbraco.resources").controller("OPTEN.Backoffice.Localization.Migration.Controller", localizationMigrationController)

	localizationMigrationController.$inject = ["$scope", "notificationsService", "OPTEN.Backoffice.Localization.Migration.Resource", '$location', '$anchorScroll'];

	function localizationMigrationController($scope, notificationsService, localizationMigrationResource, $location, $anchorScroll) {
		$scope.loading = true;
		$scope.selectedItem = {};

		$scope.loadData = function () {
			localizationMigrationResource.getContentTypes().then(function (data) {
				$scope.model = data;
				$scope.selectedItem = $scope.model[0];
				$scope.loading = false;
			});
		}

		$scope.save = function (data) {
			var properties = [];
			data.forEach(function (item, index) {
				item.properties.forEach(function (property, index) {
					if (property.localize) {
						properties.push(item.name + ": " + property.alias);
					}
				});
			});

			if (properties.length && confirm("Are you sure you want to migrate: \n" + properties.join('\n') + "?")) {
				$scope.loading = true;
				localizationMigrationResource.postContentTypes(data).then(function () {
					$scope.loadData();
				});
			}
		}

		$scope.migrateAllContentTypes = function () {
			if (confirm("Are you sure you want to migrate ALL properties?")) {
				$scope.loading = true;
				localizationMigrationResource.migrateAllContentTypes().then(function () {
					$scope.loadData();
				});
			}
		}

		$scope.goToDocType = function (target) {
			scrollToHash(target.alias);
			var el = document.getElementById(target.alias);
			var siblings = document.getElementsByClassName('content-type');

			for (var i = 0; i < siblings.length; i++) {
				siblings[i].classList.remove('-scroll-target');
			}

			el.classList.add('-scroll-target');
		}

		function scrollToHash(hash) {
			var old = $location.hash();
			$location.hash(hash);
			$anchorScroll();
			$location.hash(old);
		}

		$scope.loadData();
	};

}());