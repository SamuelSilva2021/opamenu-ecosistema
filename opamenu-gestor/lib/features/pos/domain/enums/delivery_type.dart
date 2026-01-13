enum DeliveryType {
  delivery(0, 'Delivery'),
  pickup(1, 'Retirada no Balcão'),
  table(2, 'Mesa');

  final int value;
  final String label;

  const DeliveryType(this.value, this.label);
}
