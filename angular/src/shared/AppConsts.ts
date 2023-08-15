import { ContractSettingType } from "./AppEnums";

export class AppConsts {
  static remoteServiceBaseUrl: string;
  static appBaseUrl: string;
  static appBaseHref: string; // returns angular's base-href parameter value if used during the publish
  static DEFAULT_SIGNATURE_WIDTH = 220;
  static DEFAULT_SIGNATURE_HEIGHT = 155;

  static DEFAULT_SIGNATURE_WIDTH_DIGITAL = 220;
  static DEFAULT_SIGNATURE_HEIGHT_DIGITAL = 155;

  static DEFAULT_INPUT_WIDTH = 210;
  static DEFAULT_INPUT_HEIGHT = 36;

  static localeMappings: any = [];

  static screenWidthMobile = { min: 320, max: 480 }

  static signerColor = [
    "rgb(255, 214, 91)",
    "rgb(172, 220, 230)",
    "rgb(192, 165, 207)",
    "rgb(151, 201, 191)",
    "rgb(247, 185, 148)",
    "rgb(195, 213, 230)",
    "rgb(207, 219, 127)",
  ];

  static fontFamilySignature = [
    'Qwigley', 'Italianno', 'Dancing Script', 'Alex Brush', 'Amatic SC'
  ];

  static fontFamily = [
    "Be Vietnam Pro",
    "Times New Roman",
    "Arial",
    "Monospace",
  ];

  static contractInput = {
    width: 12,
    height: 35,
  };

  static defaultFontSize = 0;

  static fontSizeSignatureTabType = 60;
  static fontSize = [16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
  static fontSizePageSignatureMobile = [20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50];
  static fontSizePageSignature = [50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80];

  static fontColor = [
    { name: "black", value: "#000" },
    { name: "red", value: "#dc3545" },
    { name: "green", value: "#198754" },
    { name: "blue", value: "#0d6efd" },
    { name: "yellow", value: "#ffc107" },
  ];

  static colorBasic = ['#212121', '#0064fa', '#f1283b']

  static readonly userManagement = {
    defaultAdminUserName: "admin",
  };

  static readonly localization = {
    defaultLocalizationSourceName: "EContract",
  };

  static readonly authorization = {
    encryptedAuthTokenName: "enc_auth_token",
  };

  static language = {}

  static signatureFormatAllowed = ['image/jpeg', 'image/jpg', 'image/png'];
  static maximumSizeofSignatureImage = 300;

  static signatureTypeList = [
    {
      id: ContractSettingType.Electronic,
      icon: "electronic",
      label: "Signature",
    },
    {
      id: ContractSettingType.Digital,
      icon: "digital",
      label: "USBToken",
    },
    {
      id: ContractSettingType.Stamp,
      icon: "stamp",
      label: "Stamp",
    },
  ];

  static otherTypeList = [
    {
      id: ContractSettingType.Text,
      icon: "text",
      label: "Text",
    },
    {
      id: ContractSettingType.DatePicker,
      icon: "datePicker",
      label: "Date",
    },
  ];
}
