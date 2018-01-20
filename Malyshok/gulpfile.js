/// <binding />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    less = require("gulp-less"); // добавляем модуль less

var paths = {
    webroot: "./Content/"
};
//  регистрируем задачу по преобразованию styles.less в файл css
gulp.task("comm", function () {
    return gulp.src('content/css/commonless.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css'))
});

gulp.task("blue", function () {
    return gulp.src('content/css/theme/blue.less')
               .pipe(less())
               .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("purple", function () {
    return gulp.src('content/css/theme/purple.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("turquoise", function () {
    return gulp.src('content/css/theme/turquoise.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("green", function () {
    return gulp.src('content/css/theme/green.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});
gulp.task("green_portal", function () {
    return gulp.src('content/css/theme/green_portal.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + 'css/theme'))
});