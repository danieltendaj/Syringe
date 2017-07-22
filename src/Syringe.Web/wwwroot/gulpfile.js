'use strict';
var gulp = require('gulp');
var sass = require('gulp-sass');
var cleanCss = require('gulp-clean-css');
var del = require('del');
var vinylPaths = require('vinyl-paths');

var concat = require('gulp-concat');
var rename = require('gulp-rename');
var minify = require('gulp-minify');

gulp.task('default', ['compile-sass', 'combify-javascript']);

// npm i -g typescript
// npm install --global gulp-cli --loglevel=error
gulp.task('compile-sass', function () {
	return gulp.src('./css/Syringe.scss')
		.pipe(sass({ includePaths: ['node_modules'] }).on('error', sass.logError))
		.pipe(vinylPaths(del))
		.pipe(cleanCss())
		.pipe(gulp.dest('./css/'));
});

gulp.task('combify-javascript', function () {
	var sourceFiles = [
		"./scripts/jquery-1.11.3.min.js",
		"./scripts/jquery-ui.1.11.4.min.js",
		"./scripts/jquery.validate.min.js",
		"./scripts/jquery.validate.unobtrusive.min.js",
		"./scripts/jquery.textcomplete-1.6.1.min.js",
		"./scripts/jquery.overlay-0.0.5.min.js",
		"./scripts/jquery.tag-it.2.0.0.min.js",
		"./scripts/sortable-1.4.2.min.js",
		"./scripts/bootstrap.min.js",
		"./scripts/material.js",
		"./scripts/ripples.js",
		"./scripts/bootbox.min.js",
		"./scripts/clipboard.min.js"
	];

	gulp.src(sourceFiles)
		.pipe(concat('external-libraries.min.js'))
		.pipe(gulp.dest('./scripts/'))
		.pipe(minify())
		.pipe(rename('external-libraries.min.js'))
		.pipe(gulp.dest('./scripts/'));
});

gulp.task('sass:watch', function () {
	gulp.watch('./css/Syringe.scss', ['sass']);
});