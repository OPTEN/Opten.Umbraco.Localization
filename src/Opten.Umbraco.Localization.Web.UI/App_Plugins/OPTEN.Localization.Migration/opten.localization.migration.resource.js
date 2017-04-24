; (function () {
	'use strict';

	angular.module("umbraco.resources").factory("OPTEN.Backoffice.Localization.Migration.Resource", localizationMigrationResource);

	localizationMigrationResource.$inject = ["$http", "umbRequestHelper"];

	function localizationMigrationResource($http, umbRequestHelper) {
		var resource = {
			getContentTypes: getContentTypes,
			postContentTypes: postContentTypes,
			migrateAllContentTypes: migrateAllContentTypes
		};

		return resource;


		// Private functions

		function getContentTypes() {
			return umbRequestHelper.resourcePromise(
				$http.get("/umbraco/backoffice/OPTEN/MigrationApi/GetAllContentTypes"),
				"Failed to retrieve content types"
			);
		};

		function postContentTypes(contentTypes) {
			return umbRequestHelper.resourcePromise(
				 $http({
				 	url: "/umbraco/backoffice/OPTEN/MigrationApi/PostLocalize",
				 	method: "POST",
				 	data: contentTypes
				 }),
				"Failed to migrate content types"
			);
		};

		function migrateAllContentTypes() {
			return umbRequestHelper.resourcePromise(
				$http.post("/umbraco/backoffice/OPTEN/MigrationApi/PostLocalizeAll"),
				"Failed to migrate content types"
			);
		}
	};

}());