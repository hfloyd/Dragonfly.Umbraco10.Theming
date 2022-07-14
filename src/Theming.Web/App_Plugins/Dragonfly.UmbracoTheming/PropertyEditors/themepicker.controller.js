angular.module("umbraco")
    .controller("ThemePickerController",
    function ($scope, umbRequestHelper, $http) {

    //var url = Umbraco.Sys.ServerVariables["Theming"]["ThemingPropertyEditorsBaseUrl"] + "GetThemes";
        var url = "/Umbraco/backoffice/Dragonfly/ThemePropertyEditor/GetThemes";

    umbRequestHelper.resourcePromise(
            $http.get(url),
            'Failed to retrieve themes for Site at ' + url)
        .then(function (data) {
            $scope.themes = data;
        });

});