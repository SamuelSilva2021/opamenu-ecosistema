import 'result.dart';

class SuccessResult<T> extends Result<T> {
  final T value;

  const SuccessResult(this.value);
}

