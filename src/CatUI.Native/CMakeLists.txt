add_library (CatUI.Native SHARED "windowing.cpp" "windowing.h" "utils.h" "egl_context_manager.cpp" "egl_context_manager.h")

find_package(glfw3 CONFIG REQUIRED)
target_link_libraries(CatUI.Native PRIVATE glfw)

find_package(unofficial-angle CONFIG REQUIRED)
target_link_libraries(CatUI.Native PRIVATE unofficial::angle::libEGL unofficial::angle::libGLESv2)
#target_include_directories(CatUI.Native PRIVATE include)

set_property(TARGET CatUI.Native PROPERTY CXX_STANDARD 20)