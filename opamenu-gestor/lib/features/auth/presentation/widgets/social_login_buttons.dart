import 'package:flutter/material.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:opamenu_gestor/core/constants/app_strings.dart';

class SocialLoginButtons extends StatelessWidget {
  const SocialLoginButtons({super.key});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton.icon(
            onPressed: () {},
            icon: const Icon(FontAwesomeIcons.google, color: Colors.red, size: 20),
            label: Text(AppStrings.google),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: OutlinedButton.icon(
            onPressed: () {},
            icon: const Icon(FontAwesomeIcons.facebookF, color: Colors.blue, size: 20),
            label: Text(AppStrings.facebook),
          ),
        ),
      ],
    );
  }
}
