module.exports = function(grunt) {

	// Project configuration.
	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
		concat: {
			options: {
				// define a string to put between each file in the concatenated output
				separator: ';'
			},
			dist: {
				// the files to concatenate
				src: ['Scripts/Game/**/*.js'],
				// the location of the resulting JS file
				dest: 'Scripts/<%= pkg.name %>.js'
				}
		},
		uglify: {
			options: {
				banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd-ss") %> */\n'
			},
			build: {
				src: 'Scripts/<%= pkg.name %>.js',
				dest: 'Scripts/<%= pkg.name %>.min.js'
			}
		},
		clean: {
			build: {
				src: [
				"Scripts/<%= pkg.name %>.js", 
				"Scripts/Game/Client",
				"Scripts/Game/Networking",
				"Scripts/Game/States",
				"Scripts/Game/Toolbox",
				"Scripts/Game/Main.js"],
			}
		},
		jshint: {
			// define the files to lint
			files: ['gruntfile.js', 'Scripts/Game/**/*.js'],
			// configure JSHint (documented at http://www.jshint.com/docs/)
			options: {
			// more options here if you want to override JSHint defaults
				globals: {
					jQuery: true,
					console: true,
					module: true
				}
			}
		}
	});

	// Load the plugin that provides the tasks.
	grunt.loadNpmTasks('grunt-contrib-concat');
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-contrib-clean');
	//grunt.loadNpmTasks('grunt-contrib-jshint');

	// Default task(s).
	grunt.registerTask('default', ['concat', 'uglify','clean']);
	
};