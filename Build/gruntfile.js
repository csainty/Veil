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
            options: {
                projectConfiguration: 'Release',
                target: ['Clean', 'Rebuild'],
                stdout: true,
                buildParameters: {
                    platform: 'AnyCPU'
                }
            },
            core: {
                src: ['../src/Veil/Veil.csproj'],
                options: {
                    buildParameters: {
                        documentationFile: 'Veil.xml'
                    }
                }
            },
            supersimple: {
                src: ['../src/Veil.SuperSimple/Veil.SuperSimple.csproj']
            },
            handlebars: {
                src: ['../src/Veil.Handlebars/Veil.Handlebars.csproj']
            },
            nancy: {
                src: ['../src/Nancy.ViewEngines.Veil/Nancy.ViewEngines.Veil.csproj']
            },
            mvc: {
                src: ['../src/Veil.Mvc5/Veil.Mvc5.csproj']
            },
            bench: {
                src: ['../src/Veil.Benchmark/Veil.Benchmark.csproj']
            },
            tests: {
                src: ['../src/Veil.Tests/Veil.Tests.csproj']
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
                    src: ['*.nuspec'],
                    dest: 'dist/'
                }]
            },
            lib: {
                files: [{
                    expand: true,
                    cwd: '../src/Veil/bin/Release/',
                    src: ['Veil.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil/',
                    src: ['Veil.xml'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil.SuperSimple/bin/Release/',
                    src: ['Veil.SuperSimple.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil.Handlebars/bin/Release/',
                    src: ['Veil.Handlebars.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Nancy.ViewEngines.Veil/bin/Release/',
                    src: ['Nancy.ViewEngines.Veil.*'],
                    dest: 'dist/lib/net40'
                },{
                    expand: true,
                    cwd: '../src/Veil.Mvc5/bin/Release/',
                    src: ['Veil.Mvc5.*'],
                    dest: 'dist/lib/net45'
                }]
            }
        },
        nugetpack: {
            options: {
                version: pkg.version
            },
            core: {
                src: 'dist/Veil.nuspec',
                dest: 'dist/'
            },
            supersimple: {
                src: 'dist/Veil.SuperSimple.nuspec',
                dest: 'dist/'
            },
            handlebars: {
                src: 'dist/Veil.Handlebars.nuspec',
                dest: 'dist/'
            },
            nancy: {
                src: 'dist/Nancy.ViewEngines.Veil.nuspec',
                dest: 'dist/'
            },
            mvc: {
                src: 'dist/Veil.Mvc5.nuspec',
                dest: 'dist/'
            }
        },
        nugetpush: {
            dist: {
                src: 'dist/*.nupkg'
            }
        },
        nugetrestore: {
            restore: {
                src: '../src/**/packages.config',
                dest: '../src/packages/'
            }
        }
    });
    grunt.loadNpmTasks('grunt-nuget');
    grunt.loadNpmTasks('grunt-msbuild');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-dotnet-assembly-info');
    grunt.registerTask("build", ["nugetrestore", "msbuild"])
    grunt.registerTask("default", ["clean", "assemblyinfo", "build", "copy", "nugetpack"]);
    grunt.registerTask("push", ["default", "nugetpush"]);
};
