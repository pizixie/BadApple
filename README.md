# BadApple

协议部分参考[libimobiledevice](https://github.com/libimobiledevice/libimobiledevice)<br>
lockdown部分参考[MobileDevice](https://github.com/nivalxer/MobileDevice)，使用iTunesMobileDevice.dll，这与libimobiledevice不同。

功能进度：
1. springboard
2. 设备信息
3. app安装卸载
4. 设备同步
    * 联系人
    * ~音乐、视频等~

项目依赖plist、[CoreFoundtion](https://github.com/pizixie/CoreFoundation)的分支

已处理问题：
解决了CFString不支持中文的问题。
