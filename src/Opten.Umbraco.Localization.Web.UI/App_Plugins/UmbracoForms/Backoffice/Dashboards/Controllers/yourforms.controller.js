angular.module("umbraco").controller("UmbracoForms.Dashboards.YourFormsController", function ($scope,$location, formResource, recordResource, userService, securityResource) {

    $scope.formsLimit = 4;

    $scope.showAll = function(){
        $scope.formsLimit = 50;
    };

    formResource.getOverView().then(function(response){
        $scope.forms = response.data;

        _.each($scope.forms, function(form){
            var filter = { form: form.id };

            recordResource.getRecordsCount(filter).then(function(response){
                    form.entries = response.data.count;
            });
        });
    });
});
