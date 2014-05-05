module.exports = function (grunt) {
    var pkg = grunt.file.readJSON("package.json");

    grunt.initConfig({
        pkg: pkg,
        assemblyinfo: {
            options: {
                files: ['../Src/Veil.sln'],

                // Standard assembly info
                info: {
                    version: pkg.assemblyVersion, 
                    fileVersion: pkg.assemblyVersion
                }
            }
        },        
        msbuild: {
            core: {
                src: ['../src/Veil/Veil.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true,
                    buildParameters: {
                        documentationFile: 'Veil.xml'
                    }
                }
            }
        },
        clean: {
            dist: {
                src: ["./dist"]
            }
        },        
        copy: {
            nuspec: {
                files: [{
                    expand: true,
                    cwd: '../Src/Nuspec/',
                    src: ['Veil.nuspec'],
                    dest: 'dist/'
                }]
            },
            lib: {
                files: [{
                    expand: true,
                    cwd: '../src/Veil/bin/Release/',
                    src: ['Veil.dll', 'Veil.pdb', 'Veil.xml'],
                    dest: 'dist/lib/net40'
                }]
            }
        },
        nugetpack: {
            dist: {
                src: 'dist/Veil.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }
            }
        }
    });
    grunt.loadNpmTasks('grunt-nuget');
    grunt.loadNpmTasks('grunt-msbuild');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-dotnet-assembly-info');
 
    grunt.registerTask("default", ["clean", "assemblyinfo", "msbuild", "copy:nuspec", "copy:lib", "nugetpack:dist"]);
};