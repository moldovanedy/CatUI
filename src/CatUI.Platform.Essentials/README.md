# CatUI.Platform.Essentials

Contains all the functionality that MUST be implemented on each platform where CatUI runs. This is generally like an
abstract interface, where functions must be implemented or called in a platform-specific manner.

If you want to use CatUI where an implementation is missing, you need to at least implement the functionality from this
package. 