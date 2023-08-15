import * as FileSaver from 'file-saver';

export function converFile(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }
  export function downloadFile(fileByte, fileName:string) {
    const file = new Blob([this.converFile(atob(fileByte))], {
      type: ""
    });
    FileSaver.saveAs(file, fileName);
  }
