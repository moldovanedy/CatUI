wl_dep = dependency('wayland-client')

catui_library = library(
    'catui',
    ['main_handler.cpp'],
    version: version_string,
    soversion: '1',
    include_directories: include,
    dependencies: [wl_dep]
)