function UserCtrl($scope, $http) {

    $http.get('api/user/getuser').success(function (data) {
        $scope.user = data;
    });

    $scope.suscribeFeed = function () {
        var feedURL = $scope.feedURL;

        $http({
            method: 'POST',
            url: 'api/user/feed',
            data: $.param({ "": feedURL }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).success(function (data) {
            $scope.user.Feeds.push(categoryName);
            $scope.tagName = '';
        });
    }

    $scope.unsuscribeFeed = function (feedId) {
        $http({
            method: 'DELETE',
            url: 'api/user/feed/' + feedId,
        }).success(function (data) {
            $scope.user.Feeds.splice($scope.user.Feeds.indexOf(feedId), 1);
        });
    }

    $scope.removeTag = function (tagName) {
        $http({
            method: 'DELETE',
            url: 'api/user/category/' + tagName,
        }).success(function (data) {
            $scope.user.Tags.splice($scope.user.Tags.indexOf(tagName), 1);
        });
    }

    $scope.addTag = function () {
        var categoryName = $scope.tagName;

        $http({
            method: 'POST',
            url: 'api/user/category',
            data: $.param({"": categoryName}),
            headers: {'Content-Type': 'application/x-www-form-urlencoded'}
        }).success(function (data) {
            $scope.user.Tags.push(categoryName);
            $scope.tagName = '';
        });

        /*
        $http.post('api/user/category', { "": categoryName }).success(function (data) {
            $scope.user.Tags.push(categoryName);
            $scope.todoText = '';
        });
        */
    };

}

function ConfigurationController($scope, $http, $route, $routeParams, $location) {
}

function MainController($scope, $http, $route, $routeParams, $location) {

    $scope.$location = $location;

}

function NewsController($scope, $http, $route, $routeParams, $location) {
    var newsURL;
    var data;

    if ($routeParams.category == 'all') {
         newsURL = '/api/new/getnews';
    }
    else {
        newsURL = '/api/new/getnewsbytag/' + $routeParams.category;
    }

    $http.get(newsURL).success(function (data) {
        $scope.news = data;
    });
}
