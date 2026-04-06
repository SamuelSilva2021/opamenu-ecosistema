import '../failures/failure.dart';
import 'result.dart';

class FailureResult<T> extends Result<T> {
  final Failure failure;

  const FailureResult(this.failure);
}

