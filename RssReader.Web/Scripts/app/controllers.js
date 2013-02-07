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
            data: $.param({ "": categoryName }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
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

function SuscriptionController($scope, $http, $route, $routeParams, $location, userService) {

    /*
    $http.get('api/user/getuser').success(function (data) {
        $scope.feeds = data.Feeds;
    });
    */

    userService.get(function (data) {
        $scope.feeds = data.Feeds;
    });

    $scope.suscribeFeed = function () {
        var feedURL = $scope.feedURL;

        $http({
            method: 'POST',
            url: 'api/user/feed',
            data: $.param({ "": feedURL }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).success(function (data) {
            $scope.feeds.push(data);
            $scope.feedURL = '';
        });
    }

    $scope.unsuscribeFeed = function (feedId) {
        $http({
            method: 'DELETE',
            url: 'api/user/feed/' + feedId,
        }).success(function (data) {
            $scope.feeds.splice($scope.feeds.indexOf(feedId), 1);
        });
    }

}

function MainController($scope, $http, $route, $routeParams, $location) {
}

function NewsController($scope, $http, $route, $routeParams, $location, userService) {
    
    var tags;

    userService.get(function (data) {
        $scope.tags = data.Tags;
        //tags = data.Tags;
    });

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

        /*
        for (var ii = 0; ii < $scope.news.length; ii++) {
            $scope.$watch('news[' + ii + ']', function (changed) {
                
                alert(changed.text + " " + changed.done);
            }, true);
        }*/
    });
    
    
    /*
    $scope.$watch('selectedTag', function (newValue, oldValue) 
    { 
        //scope.counter = scope.counter + 1; ;
        if (newValue) {
            alert('selecciono ' + newValue);
        }
    });
    

    $scope.selected = function (selectedOption) {
        
        alert('selecciono: ' + selectedOption);
        
    };
    */

    $scope.example = function () {
        alert('bla');
    }

    $scope.selectAction = function () {
        alert($scope.selectedTag);
    }
    /*
    $scope.version2 = {
        query: function (query) {
            var data = { results: [] };
            var existingTags = $scope.new.Tags;

      
            angular.forEach(tags, function (item, key) {
                if (query.term.toUpperCase() === item.substring(0, query.term.length).toUpperCase()) {
                    data.results.push(item);
                }
            });
      
            query.callback(data);
        }
    };
    */
}
