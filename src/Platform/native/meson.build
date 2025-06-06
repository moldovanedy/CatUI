project(
    'CatUI Native wrapper', 'cpp', 
    default_options : [
        'c_std=c14', 
        'cpp_std=c++14',
        'buildtype=debug'
    ]
)

include = include_directories('include')
version_string = '0.1.0'

if host_machine.system() == 'windows'
    subdir('src/windows')
else
    error('Unsupported platform')
endif
