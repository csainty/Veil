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
                        documentationFile: 'Veil.xml',
                        platform: 'AnyCPU'
                    }
                }
            },
            supersimple: {
                src: ['../src/Veil.SuperSimple/Veil.SuperSimple.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true,
                    buildParameters: {
                        platform: 'AnyCPU'
                    }
                }
            },
            handlebars: {
                src: ['../src/Veil.Handlebars/Veil.Handlebars.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true,
                    buildParameters: {
                        platform: 'AnyCPU'
                    }
                }
            },
            nancy: {
                src: ['../src/Nancy.ViewEngines.Veil/Nancy.ViewEngines.Veil.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true,
                    buildParameters: {
                        platform: 'AnyCPU'
                    }
                }
            },
            bench: {
                src: ['../src/Veil.Benchmark/Veil.Benchmark.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    target: ['Clean', 'Rebuild'],
                    stdout: true,
                    buildParameters: {
                        platform: 'AnyCPU'
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
                }]
            }
        },
        nugetpack: {
            core: {
                src: 'dist/Veil.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }
            },
            supersimple: {
                src: 'dist/Veil.SuperSimple.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }                
            },
            handlebars: {
                src: 'dist/Veil.Handlebars.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }                
            },
            nancy: {
                src: 'dist/Nancy.ViewEngines.Veil.nuspec',
                dest: 'dist/',
                options: {
                    version: pkg.version
                }                
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
