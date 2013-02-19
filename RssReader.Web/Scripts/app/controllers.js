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

    userService.getFeeds(function (data) {
        $scope.feeds = data;
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

function MainController($scope, $http, $route, $routeParams, $location, userService) {
    userService.getTags(function (data) {
        $scope.tags = data;
    });
}

function NewsController($scope, $http, $route, $routeParams, $location, userService, newsService, tagService) {
        
    userService.getTags(function (data) {
        data.push("<b>New Tag</b>");
        $scope.tags = data;
    });
                        
    if ($routeParams.category == 'all') {
        newsService.getNews(function(data){
            $scope.news = data;
        });        
    }
    else {
        newsService.getNewsByTag($routeParams.category, function(data){
            $scope.news = data;
        });        
    }

    $scope.selectTagAction = function (newIndex) {
        var currentNew = $scope.news[newIndex];
        currentNew.Tags.push(this.selectedTag);

        newsService.tagNew(currentNew.Id, this.selectedTag);

        this.selectedTag = null;
    }

    $scope.removetag = function (newIndex, tagToRemove) {
        var currentNew = $scope.news[newIndex];                
        currentNew.Tags.splice(currentNew.Tags.indexOf(tagToRemove), 1);

        newsService.untagNew(currentNew.Id, tagToRemove);
    }    
}
