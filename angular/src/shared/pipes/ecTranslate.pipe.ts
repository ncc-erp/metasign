import { Pipe, PipeTransform,Injector,Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/app-component-base';

@Injectable({ providedIn: 'root' })
@Pipe({
  name: 'ecTranslate',
  
  
})
export class EcTranslatePipe  implements PipeTransform {
  transform(value: string): string {

    if(value === null) {
      return ''
    }
    const words = value?.split(' '); 
    const translatedWords = words?.map(word => {
      const translatedWord = AppConsts.language[word];
      return translatedWord ? translatedWord : word; 
    });
    return translatedWords?.join(' '); 
  }
}