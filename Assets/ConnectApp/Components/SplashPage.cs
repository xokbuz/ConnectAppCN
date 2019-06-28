using System;
using System.Collections.Generic;
using ConnectApp.Constants;
using ConnectApp.Main;
using ConnectApp.Plugins;
using ConnectApp.screens;
using ConnectApp.Utils;
using Unity.UIWidgets.async;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using Image = Unity.UIWidgets.widgets.Image;

namespace ConnectApp.Components {
    public class SplashPage : StatefulWidget {
        public override State createState() {
            return new _SplashPageState();
        }
    }

    class _SplashPageState : State<SplashPage> {
        bool _isShow;
        Timer _timer;
        int _lastSecond = 5;
        BuildContext _context;

        public override void initState() {
            base.initState();
            StatusBarManager.hideStatusBar(true);
            this._isShow = SplashManager.isExistSplash();
            if (this._isShow) {
                this._lastSecond = SplashManager.getSplash().duration;
                this._timer = Window.instance.run(TimeSpan.FromSeconds(1), this.t_Tick, true);
            }
        }

        public override void dispose() {
            this._timer?.Dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context) {
            this._context = context;
            if (!this._isShow) {
                return new MainScreen();
            }

            var topPadding = 0f;
            if (Application.platform != RuntimePlatform.Android) {
                topPadding = MediaQuery.of(context).padding.top;
            }

            return new Container(
                color: CColors.White,
                child: new Stack(
                    children: new List<Widget> {
                        new Column(
                            children: new List<Widget> {
                                new GestureDetector(
                                    child: new Container(
                                        width: MediaQuery.of(context).size.width,
                                        height: MediaQuery.of(context).size.height,
                                        child: Image.memory(SplashManager.readImage(), fit: BoxFit.cover)
                                    ),
                                    onTap: pushPage
                                )
                            }
                        ),
                        new Positioned(
                            top: topPadding + 24,
                            right: 16,
                            child: new GestureDetector(
                                child: new Container(
                                    decoration: new BoxDecoration(
                                        Color.fromRGBO(0, 0, 0, 0.5f),
                                        borderRadius: BorderRadius.all(16)
                                    ),
                                    width: 65,
                                    height: 32,
                                    alignment: Alignment.center,
                                    child: new Text($"跳过 {this._lastSecond}", style: new TextStyle(
                                        fontSize: 14,
                                        fontFamily: "PingFangSC-Regular",
                                        color: CColors.White
                                    ))
                                ),
                                onTap: pushCallback
                            )
                        )
                    }
                )
            );
        }

        static void pushPage() {
            var splash = SplashManager.getSplash();
            AnalyticsManager.ClickSplashPage(splash.id, splash.name, splash.url);
            Router.navigator.pushReplacementNamed(MainNavigatorRoutes.Main);
            JPushPlugin.openUrl(splash.url);
        }

        static void pushCallback() {
            Router.navigator.pushReplacementNamed(MainNavigatorRoutes.Main);
        }

        void t_Tick() {
            using (WindowProvider.of(this._context).getScope()) {
                this.setState(() => { this._lastSecond -= 1; });
                if (this._lastSecond < 1) {
                    pushCallback();
                }
            }
        }
    }
}