/// <binding AfterBuild='syringe.scss' />
'use strict';
var gulp = require('gulp');
var sass = require('gulp-sass');
var cleanCss = require('gulp-clean-css');
var del = require('del');
var vinylPaths = require('vinyl-paths');

gulp.task('syringe.scss', function () {
	return gulp.src('./css/Syringe.scss')
		.pipe(sass({
			includePaths: ['node_modules']
		})
			.on('error', sass.logError))
		.pipe(vinylPaths(del))
		.pipe(cleanCss())
		.pipe(gulp.dest('./css/'));
});

gulp.task('sass:watch', function () {
	gulp.watch('./css/Syringe.scss', ['sass']);
});
