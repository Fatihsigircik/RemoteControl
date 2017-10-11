function getImageFromGzipFromBase64(data) {
    // Get some base64 encoded binary data from the server. Imagine we got this:
    var b64Data = data; //'H4sIAAAAAAAAAwXB2w0AEBAEwFbWl2Y0IW4jQmziPNo3k6TuGK0Tj/ESVRs6yzkuHRnGIqPB92qzhg8yp62UMAAAAA==';
    // Decode base64 (convert ascii to binary)
    var strData = a2b(b64Data); //atob(b64Data);

    console.log(strData.length);
    // Convert binary string to character-number array
    var charData = strData.split('').map(function(x) { return x.charCodeAt(0); });

    // Turn number array into byte-array
    var binData = new Uint8Array(charData);

    // Pako magic
    var data1 = pako.inflate(binData);

    // Convert gunzipped byteArray back to ascii string:
    var strData1 = String.fromCharCode.apply(null, new Uint8Array(data1));

    // Output to console

    return strData1;
}


function b2a(a) {
    var c, d, e, f, g, h, i, j, o, b = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
        k = 0,
        l = 0,
        m = "",
        n = [];
    if (!a) return a;
    do c = a.charCodeAt(k++), d = a.charCodeAt(k++), e = a.charCodeAt(k++), j = c << 16 | d << 8 | e,
        f = 63 & j >> 18, g = 63 & j >> 12, h = 63 & j >> 6, i = 63 & j, n[l++] = b.charAt(f) + b.charAt(g) + b.charAt(h) + b.charAt(i); while (k < a.length);
    return m = n.join(""), o = a.length % 3, (o ? m.slice(0, o - 3) : m) + "===".slice(o || 3);
}

function a2b(a) {
    var b, c, d, e = {},
        f = 0,
        g = 0,
        h = "",
        i = String.fromCharCode,
        j = a.length;
    for (b = 0; 64 > b; b++) e["ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".charAt(b)] = b;
    for (c = 0; j > c; c++)
        for (b = e[a.charAt(c)], f = (f << 6) + b, g += 6; g >= 8;)((d = 255 & f >>> (g -= 8)) || j - 2 > c) && (h += i(d));
    return h;
}