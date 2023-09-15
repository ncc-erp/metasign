import { Injector, Pipe, PipeTransform } from "@angular/core";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/app-component-base";

@Pipe({
  name: "localize",
})
export class LocalizePipe extends AppComponentBase implements PipeTransform {
  constructor(injector: Injector) {
    super(injector);
  }

  transform(value: string): string {
    if (value === null) {
      return "";
    }
    const words = value.split(" ");
    const translatedWords = words.map((word) => {
      const translatedWord = AppConsts.language[word];
      return translatedWord ? translatedWord : word;
    });
    return translatedWords.join(" ");
  }
}
