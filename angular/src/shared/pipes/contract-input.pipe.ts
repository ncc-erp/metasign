import { Injector, Pipe, PipeTransform } from "@angular/core";
import { ContractSettingType } from "@shared/AppEnums";
import { AppComponentBase } from "@shared/app-component-base";

@Pipe({
  name: "contractinput",
})
export class ContractInputPipe
  extends AppComponentBase
  implements PipeTransform
{
  constructor(injector: Injector) {
    super(injector);
  }

  transform(value: ContractSettingType): string {
    switch (value) {
      case ContractSettingType.Text:
        return "Text";
      case ContractSettingType.DatePicker:
        return "Date";
      case ContractSettingType.Electronic:
        return "Signature";
      case ContractSettingType.Digital:
        return "USBToken";
      case ContractSettingType.Stamp:
        return "Stamp";
    }
  }
}
