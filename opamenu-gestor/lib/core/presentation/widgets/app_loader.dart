import 'package:flutter/material.dart';
import '../../theme/app_colors.dart';

class AppLoader extends StatelessWidget {
  final Color? color;
  final double size;

  const AppLoader({super.key, this.color, this.size = 40.0});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: SizedBox(
        height: size,
        width: size,
        child: CircularProgressIndicator(
          valueColor: AlwaysStoppedAnimation<Color>(color ?? AppColors.primary),
          strokeWidth: 3,
        ),
      ),
    );
  }
}

class LoadingOverlay {
  static bool _visible = false;
  static Future<void> show(BuildContext context, {String? message}) async {
    if (_visible) return;
    _visible = true;
    await showDialog(
      context: context,
      barrierDismissible: false,
      barrierColor: Colors.black54,
      useRootNavigator: true,
      builder: (_) => WillPopScope(
        onWillPop: () async => false,
        child: Center(
          child: Card(
            elevation: 8,
            shape: const RoundedRectangleBorder(borderRadius: BorderRadius.all(Radius.circular(16))),
            child: Padding(
              padding: const EdgeInsets.all(24.0),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const AppLoader(),
                  if (message != null) ...[
                    const SizedBox(height: 16),
                    Text(message, style: const TextStyle(fontWeight: FontWeight.w500)),
                  ],
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  static void hide(BuildContext context) {
    final navigator = Navigator.of(context, rootNavigator: true);
    if (navigator.canPop()) {
      navigator.pop();
    }
    _visible = false;
  }
}
