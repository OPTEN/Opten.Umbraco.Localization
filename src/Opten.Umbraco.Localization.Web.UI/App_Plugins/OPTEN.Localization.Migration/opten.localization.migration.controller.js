; (function () {
	'use strict';

	angular.module("umbraco.resources").controller("OPTEN.Backoffice.Localization.Migration.Controller", localizationMigrationController)

	localizationMigrationController.$inject = ["$scope", "notificationsService", "OPTEN.Backoffice.Localization.Migration.Resource"];

	function localizationMigrationController($scope, notificationsService, localizationMigrationResource) {
		$scope.loading = true;

		$scope.loadData = function () {
			localizationMigrationResource.getContentTypes().then(function (data) {
				$scope.model = data;
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

		$scope.loadData();
	};

}());