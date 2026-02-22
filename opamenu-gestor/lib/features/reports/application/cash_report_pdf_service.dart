import 'package:flutter/services.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:printing/printing.dart';
import 'package:intl/intl.dart';
import '../../pos/domain/models/cash_register_report_dto.dart';

class CashReportPdfService {
  static Future<void> generateAndShare(CashRegisterReportDto report) async {
    final pdf = pw.Document();
    final currencyFormat = NumberFormat.currency(locale: 'pt_BR', symbol: 'R\$');
    final dateFormat = DateFormat('dd/MM/yyyy HH:mm');

    pdf.addPage(
      pw.MultiPage(
        pageFormat: PdfPageFormat.a4,
        margin: const pw.EdgeInsets.all(32),
        build: (context) => [
          _buildHeader(report, dateFormat),
          pw.SizedBox(height: 24),
          _buildKpiSummary(report, currencyFormat),
          pw.SizedBox(height: 32),
          _buildPaymentMethodsTable(report, currencyFormat),
          pw.SizedBox(height: 32),
          _buildConsiliationSummary(report, currencyFormat),
          pw.Padding(
            padding: const pw.EdgeInsets.only(top: 48),
            child: pw.Center(
              child: pw.Text(
                'Relatório gerado em ${dateFormat.format(DateTime.now())}',
                style: const pw.TextStyle(color: PdfColors.grey700, fontSize: 10),
              ),
            ),
          ),
        ],
      ),
    );

    await Printing.sharePdf(
      bytes: await pdf.save(),
      filename: 'relatorio_caixa_${DateFormat('yyyyMMdd').format(report.startDate)}.pdf',
    );
  }

  static pw.Widget _buildHeader(CashRegisterReportDto report, DateFormat dateFormat) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.start,
      children: [
        pw.Text('Opamenu - Relatório de Caixa',
            style: pw.TextStyle(fontSize: 24, fontWeight: pw.FontWeight.bold, color: PdfColors.blue900)),
        pw.SizedBox(height: 8),
        pw.Text('Período: ${dateFormat.format(report.startDate)} até ${dateFormat.format(report.endDate)}',
            style: const pw.TextStyle(fontSize: 14)),
        pw.Divider(thickness: 2, color: PdfColors.blue900),
      ],
    );
  }

  static pw.Widget _buildKpiSummary(CashRegisterReportDto report, NumberFormat currencyFormat) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.start,
      children: [
        pw.Text('Resumo Financeiro', style: pw.TextStyle(fontSize: 18, fontWeight: pw.FontWeight.bold)),
        pw.SizedBox(height: 16),
        pw.Row(
          mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
          children: [
            _buildStatBox('Venda Bruta', currencyFormat.format(report.totalGrossSales)),
            _buildStatBox('Venda Líquida', currencyFormat.format(report.totalNetSales)),
          ],
        ),
        pw.SizedBox(height: 16),
        pw.Row(
          mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
          children: [
            _buildStatBox('Total Entradas', currencyFormat.format(report.totalInflows)),
            _buildStatBox('Total Saídas', currencyFormat.format(report.totalOutflows)),
          ],
        ),
      ],
    );
  }

  static pw.Widget _buildStatBox(String label, String value) {
    return pw.Container(
      width: 240,
      padding: const pw.EdgeInsets.all(12),
      decoration: pw.BoxDecoration(
        border: pw.Border.all(color: PdfColors.grey400),
        borderRadius: const pw.BorderRadius.all(pw.Radius.circular(8)),
      ),
      child: pw.Column(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Text(label, style: const pw.TextStyle(fontSize: 12, color: PdfColors.grey700)),
          pw.SizedBox(height: 4),
          pw.Text(value, style: pw.TextStyle(fontSize: 18, fontWeight: pw.FontWeight.bold)),
        ],
      ),
    );
  }

  static pw.Widget _buildPaymentMethodsTable(CashRegisterReportDto report, NumberFormat currencyFormat) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.start,
      children: [
        pw.Text('Vendas por Método de Pagamento', style: pw.TextStyle(fontSize: 18, fontWeight: pw.FontWeight.bold)),
        pw.SizedBox(height: 16),
        pw.Table(
          border: pw.TableBorder.all(color: PdfColors.grey400),
          children: [
            pw.TableRow(
              decoration: const pw.BoxDecoration(color: PdfColors.grey200),
              children: [
                pw.Padding(padding: const pw.EdgeInsets.all(8), child: pw.Text('Método', style: pw.TextStyle(fontWeight: pw.FontWeight.bold))),
                pw.Padding(padding: const pw.EdgeInsets.all(8), child: pw.Text('Qtd', style: pw.TextStyle(fontWeight: pw.FontWeight.bold))),
                pw.Padding(padding: const pw.EdgeInsets.all(8), child: pw.Text('Valor Total', style: pw.TextStyle(fontWeight: pw.FontWeight.bold), textAlign: pw.TextAlign.right)),
              ],
            ),
            ...report.salesByPaymentMethod.map((item) => pw.TableRow(
              children: [
                pw.Padding(padding: const pw.EdgeInsets.all(8), child: pw.Text(item.paymentMethod.displayName)),
                pw.Padding(padding: const pw.EdgeInsets.all(8), child: pw.Text(item.count.toString())),
                pw.Padding(padding: const pw.EdgeInsets.all(8), child: pw.Text(currencyFormat.format(item.totalAmount), textAlign: pw.TextAlign.right)),
              ],
            )),
          ],
        ),
      ],
    );
  }

  static pw.Widget _buildConsiliationSummary(CashRegisterReportDto report, NumberFormat currencyFormat) {
    final isNegative = report.totalDiscrepancy < 0;
    
    return pw.Container(
      padding: const pw.EdgeInsets.all(16),
      decoration: pw.BoxDecoration(
        color: PdfColors.blue50,
        borderRadius: const pw.BorderRadius.all(pw.Radius.circular(8)),
      ),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text('Quebra/Sobra Total de Caixa:', style: pw.TextStyle(fontWeight: pw.FontWeight.bold, fontSize: 16)),
          pw.Text(
            currencyFormat.format(report.totalDiscrepancy),
            style: pw.TextStyle(
              fontWeight: pw.FontWeight.bold, 
              fontSize: 18,
              color: report.totalDiscrepancy == 0 ? PdfColors.black : (isNegative ? PdfColors.red : PdfColors.green)
            ),
          ),
        ],
      ),
    );
  }
}
