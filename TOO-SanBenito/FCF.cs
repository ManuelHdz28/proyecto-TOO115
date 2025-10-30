using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Class1
{
    public class FCF
    {
        [Required(ErrorMessage = "El apartado identificacion es requerido para este tipo de documento")]
        public FCFIdentificacionModel identificacion { get; set; }
        public List<FCFDocumentoRelacionadoModel> documentoRelacionado { get; set; }
        [Required(ErrorMessage = "El apartado Emisor es requerido para este tipo de documento")]
        public FCFEmisorModel emisor { get; set; }
        [Required(ErrorMessage = "El apartado Receptor es requerido para este tipo de documento")]
        public FCFReceptorModel receptor { get; set; }
        public List<FCFotrosDocumentosModel> otrosDocumentos { get; set; }
        public FCFVentaTerceroModel ventaTercero { get; set; }
        [Required(ErrorMessage = "El apartado cuerpoDocumento es requerido para este tipo de documento")]
        public List<FCFCuerpoDocumentoModel> cuerpoDocumento { get; set; }
        [Required(ErrorMessage = "El apartado Resumen es requerido para este tipo de documento")]
        public FCFResumenModel resumen { get; set; }
        public FCFExtensionModel extension { get; set; }
        public List<FCFApendiceModel> apendice { get; set; }
    }

    public class FCFIdentificacionModel
    {
        [Required(ErrorMessage = "Apartado:Identificacion,El campo version es requerido para este tipo de documento")]
        public int version { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo ambiente es requerido para este tipo de documento")]
        [StringLength(2, ErrorMessage = "Identificacion,El tamaño del campo ambiente no debe superar 2 caracteres")]
        public string ambiente { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo tipoDTE es requerido para este tipo de documento")]
        [StringLength(2, ErrorMessage = "El tamaño del campo tipoDte no debe superar 2 caracteres")]
        public string tipoDte { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo numeroControl es requerido para este tipo de documento")]
        [RegularExpression(@"^DTE-03-[A-Z0-9]{8}-[0-9]{15}$",
         ErrorMessage = "Apartado:Identificacion,El campo numeroControl no cumple con el formato necesario")]
        public string numeroControl { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo codigoGeneracion es requerido para este tipo de documento")]
        [RegularExpression(@"^[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}$",
         ErrorMessage = "Apartado:Identificacion,El campo codigoGeneracion no cumple con el formato necesario")]
        public string codigoGeneracion { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo tipoModelo es requerido para este tipo de documento")]
        public int tipoModelo { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo tipoOperacion es requerido para este tipo de documento")]
        public int tipoOperacion { get; set; }

        public int? tipoContingencia { get; set; }
        [StringLength(150, ErrorMessage = "Apartado:Identificacion,El campo motivoContin sobrepasa el tamaño permitido", MinimumLength = 1)]
        public string motivoContin { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo fecEmi es requerido para este tipo de documento")]
        [StringLength(10)]
        public string fecEmi { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo horEmi es requerido para este tipo de documento")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]?$",
          ErrorMessage = "El campo horEmi no cumple con el formato necesario")]
        public string horEmi { get; set; }
        [Required(ErrorMessage = "Apartado:Identificacion,El campo tipoMoneda es requerido para este tipo de documento")]
        [StringLength(3, ErrorMessage = "Apartado:Identificacion,El campo tipoMoneda sobrepasa el tamaño requerido para este tipo de documento")]
        public string tipoMoneda { get; set; }
        
    }
    public class FCFDocumentoRelacionadoModel
    {
        public string tipoDte { get; set; }
        [Required(ErrorMessage = "Apartado:Documento relacionado,El campo tipoDocumento es requerido para este tipo de documento")]
        public string tipoDocumento { get; set; }
        [Required(ErrorMessage = "Apartado:Documento relacionado,El campo tipoGeneracion es requerido para este tipo de documento")]
        public int tipoGeneracion { get; set; }
        [Required(ErrorMessage = "Apartado:Documento relacionado,El campo numeroDocumento es requerido para este tipo de documento")]
        [StringLength(36, ErrorMessage = "Apartado:Documento relacionado,El campo numeroDocumento sobrepasa el tamaño requerido para este tipo de documento", MinimumLength = 1)]
        public string numeroDocumento { get; set; }
        [Required(ErrorMessage = "Apartado:Documento relacionado,El campo fechaEmision es requerido para este tipo de documento")]
        public string fechaEmision { get; set; }
    }

    public class FCFEmisorModel
    {
        [Required(ErrorMessage = "Apartado: Emisor,El campo nit es requerido para este tipo de documento")]
        [RegularExpression(@"^([0-9]{14}|[0-9]{9})$",
         ErrorMessage = "Apartado: Emisor,El campo  nit no cumple con el formato necesario")]
        public string nit { get; set; }
        [Required(ErrorMessage = "Apartado: Emisor,El campo  nrc es requerido para este tipo de documento")]
        [RegularExpression(@"^[0-9]{1,8}$",
         ErrorMessage = "Apartado: Emisor,El campo  nrc no cumple con el formato necesario")]
        public string nrc { get; set; }
        [Required(ErrorMessage = "Apartado: Emisor,El campo  nombre es requerido para este tipo de documento")]
        [StringLength(250, ErrorMessage = "Apartado: Emisor,El campo  nombre no cumple la longitud esperada", MinimumLength = 3)]
        public string nombre { get; set; }
        [Required(ErrorMessage = "Apartado: Emisor,El campo  codActividad es requerido para este tipo de documento")]
        [RegularExpression(@"^[0-9]{2,6}$",
         ErrorMessage = "Apartado: Emisor,El campo  codActividad no cumple con el formato necesario")]
        public string codActividad { get; set; }
        [Required(ErrorMessage = "Apartado: Emisor,El campo  descActividad es requerido para este tipo de documento")]
        [StringLength(150, ErrorMessage = "Apartado: Emisor,El campo  descActividad no cumple la longitud esperada", MinimumLength = 1)]
        public string descActividad { get; set; }
        [StringLength(150, ErrorMessage = "Apartado: Emisor,El campo  nombreComercial no cumple la longitud esperada", MinimumLength = 1)]
        public string nombreComercial { get; set; }
        [Required(ErrorMessage = "Apartado: Emisor,El campo  tipoEstablecimiento es requerido para este tipo de documento")]
        public string tipoEstablecimiento { get; set; }

        [Required(ErrorMessage = "Apartado: Emisor,El campo  telefono es requerido para este tipo de documento")]
        [StringLength(30, ErrorMessage = "Apartado: Emisor,El campo  telefono no cumple con la longitud esperada", MinimumLength = 8)]
        public string telefono { get; set; }
        [Required(ErrorMessage = "Apartado: Emisor,El campo  correo es requerido para este tipo de documento")]
        [StringLength(100, ErrorMessage = "Apartado: Emisor,El campo  correo no cumple con la longitud esperada", MinimumLength = 3)]
        public string correo { get; set; }
        [StringLength(4, ErrorMessage = "Apartado: Emisor,El campo  codEstableMH no cumple con la longitud esperada", MinimumLength = 4)]
        public string codEstableMH { get; set; }
        [StringLength(10, ErrorMessage = "Apartado: Emisor,El campo  codEstable no cumple con la longitud esperada", MinimumLength = 1)]
        public string codEstable { get; set; }
        [StringLength(4, ErrorMessage = "Apartado: Emisor,El campo  codPuntoVentaMH no cumple con la longitud esperada", MinimumLength = 4)]
        public string codPuntoVentaMH { get; set; }
        [StringLength(15, ErrorMessage = "Apartado: Emisor,El campo  codPuntoVenta no cumple con la longitud esperada", MinimumLength = 1)]
        public string codPuntoVenta { get; set; }

        public FCFDireccionModel direccion { get; set; }
    }

    public class FCFReceptorModel
    {
        [Required(ErrorMessage = "Apartado:Receptor:El campo tipoDocumento es requerido para este tipo de documento")]
        [RegularExpression(@"^([0-9]{14}|[0-9]{9})$",
         ErrorMessage = "Apartado:Receptor:El campo  tipoDocumento no cumple con el formato necesario")]
        [StringLength(14)]
        public string tipoDocumento { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo numDocumento es requerido para este tipo de documento")]
        [RegularExpression(@"^([0-9]{14}|[0-9]{9})$",
         ErrorMessage = "Apartado:Receptor:El campo  numDocumento no cumple con el formato necesario")]
        [StringLength(20, ErrorMessage = "Apartado:Receptor:El campo  numDocumento no cumple con la longitud esperada", MinimumLength = 3)]
        public string numDocumento { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  nrc es requerido para este tipo de documento")]
        [RegularExpression(@"^[0-9]{1,8}$",
       ErrorMessage = "Apartado:Receptor:El campo  nrc no cumple con el formato necesario")]
        [StringLength(8, ErrorMessage = "Apartado:Receptor:El campo  NRC no cumple con la longitud esperada", MinimumLength = 2)]
        public string nrc { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  nombre es requerido para este tipo de documento")]
        [StringLength(250, ErrorMessage = "Apartado:Receptor:El campo  nombre no cumple con la longitud esperada", MinimumLength = 1)]
        public string nombre { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  nombre es requerido para este tipo de documento")]
        [StringLength(150, ErrorMessage = "Apartado:Receptor:El campo  nombre no cumple con la longitud esperada", MinimumLength = 1)]
        public string nombreComercial { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  codActividad es requerido para este tipo de documento")]
        [RegularExpression(@"^[0-9]{2,6}$", ErrorMessage = "Apartado:Receptor:El campo  codActividad no cumple con el formato necesario")]
        [StringLength(6, ErrorMessage = "Apartado:Receptor:El campo  codActividad no cumple con la longitud esperada", MinimumLength = 5)]
        public string codActividad { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  descActividad es requerido para este tipo de documento")]
        [StringLength(150, ErrorMessage = "Apartado:Receptor:El campo  descActividad no cumple con la longitud esperada", MinimumLength = 1)]
        public string descActividad { get; set; }
        [Required(ErrorMessage = "En receptor,el apartado direccion es requerido para este tipo de documento")]
        public FCFDireccionModel direccion { get; set; }
        [StringLength(30, ErrorMessage = "Apartado:Receptor:El campo  telefono no cumple con la longitud esperada", MinimumLength = 8)]
        public string telefono { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  correo es requerido para este tipo de documento")]
        [StringLength(100, ErrorMessage = "Apartado:Receptor:El campo  correo no correo con la longitud esperada")]
        public string correo { get; set; }
        [Required(ErrorMessage = "Apartado:Receptor:El campo  bienTitulo es requerido para este tipo de documento")]
        [StringLength(2, ErrorMessage = "Apartado:Receptor:El campo  bienTitulo no correo con la longitud esperada")]
        public string bienTitulo { get; set; }
        public FCFReceptorModel() { }

    }

    public class FCFDireccionModel
    {
        [Required(ErrorMessage = "Apartado: Direccion,El campo departamento es requerido para este tipo de documento")]
        [RegularExpression(@"^0[1-9]|1[0-4]$",
       ErrorMessage = "Apartado: Direccion,El campo departamento no cumple con el formato necesario")]
        public string departamento { get; set; }
        [Required(ErrorMessage = "Apartado: Direccion,El campo municipio es requerido para este tipo de documento")]
        [RegularExpression(@"^[0-9]{2}",
        ErrorMessage = "Apartado: Direccion,El campo municipio no cumple con el formato necesario")]
        public string municipio { get; set; }
        [Required(ErrorMessage = "Apartado: Direccion,El campo complemento es requerido")]

        [StringLength(200, ErrorMessage = "Apartado: Direccion,El campo complemento no cumple con la longitud correcta", MinimumLength = 1)]
        public string complemento { get; set; }

    }
    public class FCFotrosDocumentosModel
    {
        [Required(ErrorMessage = "Apartado: Otros documentos,el campo codDocAsociado es requerido")]
        public int codDocAsociado { get; set; }
        [StringLength(100, ErrorMessage = "Apartado: Otros documentos,el campo  descDocumento sobrepasa la longitud estaparada")]
        public string descDocumento { get; set; }
        [StringLength(300, ErrorMessage = "Apartado: Otros documentos,el campo  detalleDocumento sobrepasa la longitud estaparada")]
        public string detalleDocumento { get; set; }
        public FCFMedicoModel medico { get; set; }

    }

    public class FCFMedicoModel
    {
        [Required(ErrorMessage = "Apatado:Medico,el campo nombre es requerido")]
        [StringLength(100, ErrorMessage = "Apatado:Medico,el campo  nombre sobrepasa la longitud estaparada")]
        public string nombre { get; set; }
        [RegularExpression(@"^([0-9]{14}|[0-9]{9})$",
      ErrorMessage = "Apatado:Medico,el campo  nit no cumple con el formato necesario")]
        public string nit { get; set; }
        [StringLength(25, ErrorMessage = "Apatado:Medico,el campo  docIdenificacion no cumple con la longitud correcta", MinimumLength = 2)]
        public string docIdenificacion { get; set; }
        [Required(ErrorMessage = "Apatado:Medico,el campo  tipoServicio es requerido")]
        public int tipoServicio { get; set; }
    }

    public class FCFVentaTerceroModel
    {
        [Required(ErrorMessage = "Apartado:Venta Tercero, el campo bienTitulo es requerido para este tipo de documento")]
        [RegularExpression(@"^([0-9]{14}|[0-9]{9})$",
      ErrorMessage = "Apartado:Venta Tercero, el campo nit no cumple con el formato necesario")]
        public string nit { get; set; }
        [Required(ErrorMessage = "Apartado:Venta Tercero, el campo bienTitulo es requerido para este tipo de documento")]
        [StringLength(200, ErrorMessage = "Apartado:Venta Tercero, el campo docIdenificacion no cumple con la longitud correcta", MinimumLength = 3)]
        public string nombre { get; set; }
    }

    public class FCFCuerpoDocumentoModel
    {
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo numItem es requerido")]
        public int numItem { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo tipoItem es requerido")]
        public int tipoItem { get; set; }

        [StringLength(36, ErrorMessage = "Apartado:Cuerpo documento,el campo numeroDocumento no cumple con la longitud correcta", MinimumLength = 1)]
        public string numeroDocumento { get; set; }
        [StringLength(25, ErrorMessage = "Apartado:Cuerpo documento,el campo Código no cumple con la longitud correcta", MinimumLength = 1)]
        public string codigo { get; set; }
        [StringLength(2, ErrorMessage = "Apartado:Cuerpo documento,el campo codTributo no cumple con la longitud correcta", MinimumLength = 2)]
        public string codTributo { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo descripcion es requerido")]
        [StringLength(1000, ErrorMessage = "Apartado:Cuerpo documento,el campo descripcion no cumple con la longitud correcta")]
        public string descripcion { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo cantidad es requerido")]
        public decimal? cantidad { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo uniMedida es requerido")]
        public int uniMedida { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo precioUni es requerido")]
        public double precioUni { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo montoDescu es requerido")]
        public double montoDescu { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo ventaNoSuj es requerido")]
        public double ventaNoSuj { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo ventaExenta es requerido")]
        public double ventaExenta { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo ventaGravada es requerido")]
        public double ventaGravada { get; set; }
        public List<string> tributos { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo psv es requerido")]
        public double psv { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo noGravado es requerido")]
        public double noGravado { get; set; }
        [Required(ErrorMessage = "Apartado:Cuerpo documento,el campo ivaItem es requerido")]
        public double ivaItem { get; set; }
    }

    public class FCFResumenModel
    {
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalNoSuj es requerido")]
        public double totalNoSuj { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalExenta es requerido")]
        public double totalExenta { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalGravada es requerido")]
        public double totalGravada { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo subTotalVentas es requerido")]
        public double subTotalVentas { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo descuNoSuj es requerido")]
        public double descuNoSuj { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo descuExenta es requerido")]
        public double descuExenta { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo descuGravada es requerido")]
        public double descuGravada { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo porcentajeDescuento es requerido")]
        public double porcentajeDescuento { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalDescu es requerido")]
        public double totalDescu { get; set; }
        public List<FCFTributosModel> tributos { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo subTotal es requerido")]
        public double subTotal { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo ivaPerci1 es requerido")]
        public double ivaPerci1 { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo ivaRete1 es requerido")]
        public double ivaRete1 { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo reteRenta es requerido")]
        public double reteRenta { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo montoTotalOperacion es requerido")]
        public double montoTotalOperacion { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalNoGravado es requerido")]
        public double totalNoGravado { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalPagar es requerido")]
        public double totalPagar { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalIva es requerido")]
        public double totalIva { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo totalLetras es requerido")]
        [StringLength(200, ErrorMessage = "Apartado:Resumen,el campo totalLetras  sobrepasa el tamaño estipulado")]
        public string totalLetras { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo saldoFavor es requerido")]
        public double saldoFavor { get; set; }
        [Required(ErrorMessage = "Apartado:Resumen,el campo condicionOperacion es requerido")]
        public int condicionOperacion { get; set; }
        public List<FCFPagosModel> pagos { get; set; }
        [StringLength(100, ErrorMessage = "Apartado:Resumen,el campo numPagoElectronico sobrepasa la longitud")]
        public string numPagoElectronico { get; set; }
    }

    public class FCFPagosModel
    {
        [Required(ErrorMessage = "Apartado:Pagos,el campo codigo es requerido")]
        [RegularExpression(@"^(0[1-9]||1[0-4]||99)$",
      ErrorMessage = "El campo codigo no cumple con el formato necesario")]
        public string codigo { get; set; }
        [Required(ErrorMessage = "Apartado:Pagos,el campo montoPago es requerido")]
        public double montoPago { get; set; }
        [StringLength(50, ErrorMessage = "Apartado:Pagos,el campo referencia sobrepasa la longitud")]
        public string referencia { get; set; }
        [RegularExpression(@"^0[1-3]$",
    ErrorMessage = "Apartado:Pagos,el campo plazo no cumple con el formato necesario")]
        [StringLength(2)]
        public string plazo { get; set; }
        public int? periodo { get; set; }
    }

    public class FCFTributosModel
    {
        [Required(ErrorMessage = "Apartado:Tributo,el campo codigo es requerido")]
        [StringLength(2, ErrorMessage = "Apartado:Tributo,el campo codigo  no comple el tamaño correcta", MinimumLength = 2)]
        public string codigo { get; set; }
        [Required(ErrorMessage = "Apartado:Tributo,el campo descripcion es requerido")]
        [StringLength(150, ErrorMessage = "Apartado:Tributo,el campo descripcion  sobrepasa el tamaño correcto", MinimumLength = 2)]
        public string descripcion { get; set; }
        [Required(ErrorMessage = "Apartado:Tributo,el campo valor es requerido")]
        public double valor { get; set; }

    }

    public class FCFExtensionModel
    {
        [StringLength(100, ErrorMessage = "Apartado:Extencion, El campo nombEntrega  sobrepasa el tamaño estipulado", MinimumLength = 1)]
        public string nombEntrega { get; set; }
        [StringLength(25, ErrorMessage = "Apartado:Extencion, El campo  docuEntrega  sobrepasa el tamaño estipulado", MinimumLength = 1)]
        public string docuEntrega { get; set; }
        [StringLength(100, ErrorMessage = "Apartado:Extencion, El campo  nombRecibe  sobrepasa el tamaño estipulado", MinimumLength = 1)]
        public string nombRecibe { get; set; }
        [StringLength(25, ErrorMessage = "Apartado:Extencion, El campo  docuRecibe  sobrepasa el tamaño estipulado", MinimumLength = 1)]
        public string docuRecibe { get; set; }
        [StringLength(3000, ErrorMessage = "Apartado:Extencion, El campo  observaciones  sobrepasa el tamaño estipulado")]
        public string observaciones { get; set; }
        [StringLength(10, ErrorMessage = "Apartado:Extencion, El campo  placaVehiculo  sobrepasa el tamaño estipulado", MinimumLength = 2)]
        public string placaVehiculo { get; set; }
    }

    public class FCFApendiceModel
    {
        [Required(ErrorMessage = "Aparatado: Apendice,el campo campo es requerido")]
        [StringLength(25, ErrorMessage = "Aparatado: Apendice,el campo  campo  no estra entre el rango permitido", MinimumLength = 2)]
        public string campo { get; set; }
        [Required(ErrorMessage = "Aparatado: Apendice,el campo  etiqueta es requerido")]
        [StringLength(50, ErrorMessage = "Aparatado: Apendice,el campo  etiqueta  no estra entre el rango permitido", MinimumLength = 2)]
        public string etiqueta { get; set; }
        [Required(ErrorMessage = "Aparatado: Apendice,el campo  valor es requerido")]
        [StringLength(150, ErrorMessage = "Aparatado: Apendice,el campo  valor  no estra entre el rango permitido", MinimumLength = 1)]
        public string valor { get; set; }

    }
}
