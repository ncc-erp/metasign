import { MassType } from './../AppEnums';
import { AppComponentBase } from '@shared/app-component-base';
import { Injector, Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'contractTemplateType'
})
export class ContractTemplateTypePipe  extends AppComponentBase implements PipeTransform {

  constructor(injector: Injector) {
    super(injector);
  }

  transform(value) {
    switch (value) {
   
        case MassType.Multiple:
          return this.ecTransform("MassTemplate") ;
        case MassType.Singel:
          return this.ecTransform("SingleTemplate") ;;

     
    }
  }

}
