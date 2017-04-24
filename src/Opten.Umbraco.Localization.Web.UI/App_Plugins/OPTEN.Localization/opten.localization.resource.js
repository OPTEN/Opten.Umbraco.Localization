; (function () {
	'use strict';

	angular.module("umbraco.resources").factory("languageResource", languageResource);

	languageResource.$inject = ["$http", "umbRequestHelper"];

	function languageResource($http, umbRequestHelper) {
		var resource = {
			getAllLanguages: getAllLanguages,
			getState: getState,
			getUrlSegment: getUrlSegment,
			getUrlAvailability: getUrlAvailability,
			getSelectedLanguages: getSelectedLanguages,
			postSelectedLanguages: postSelectedLanguages
		};

		return resource;


		// Private functions

		function getAllLanguages() {
			return umbRequestHelper.resourcePromise(
				$http.get("/umbraco/backoffice/OPTEN/LocalizationApi/GetAllLanguages"),
				"Failed to retrieve languages"
			);
		};

		function getState(contentId) {
			return umbRequestHelper.resourcePromise(
				$http.get("/umbraco/backoffice/OPTEN/LocalizationApi/GetState", { params: { contentId: contentId } }),
                "Failed to retrieve state for localization"
			);
		};

		function getUrlSegment(name) {
			return umbRequestHelper.resourcePromise(
				$http.get("/umbraco/backoffice/OPTEN/LocalizationApi/GetUrlSegment", { params: { name: name } }),
                "Failed to retrieve url segment"
			);
		};

		function getUrlAvailability(contentId, url, level) {
			return umbRequestHelper.resourcePromise(
				$http.get("/umbraco/backoffice/OPTEN/LocalizationApi/GetUrlAvailability", { params: { contentId: contentId, url: url, level: level } }),
				"Failed to retrieve segment availability"
			);
		};

		function getSelectedLanguages(languages) {
			return umbRequestHelper.resourcePromise(
				$http.get("/umbraco/backoffice/OPTEN/LocalizationApi/GetSelectedLanguages"),
				"Failed to retrieve selected languages"
			);
		};

		function postSelectedLanguages(languages) {
			return umbRequestHelper.resourcePromise(
				 $http({
				 	url: "/umbraco/backoffice/OPTEN/LocalizationApi/PostSelectedLanguages",
				 	method: "POST",
				 	data: languages
				 }),
				"Failed to set selected languages"
			);
		};
	};

}());