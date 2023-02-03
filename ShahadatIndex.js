const { Client, MessageMedia, LocalAuth  } = require('whatsapp-web.js');

var parallel = require('run-parallel')
const qrcode = require('qrcode-terminal');
const XLSX = require('xlsx');

const client = new Client({
    authStrategy: new LocalAuth(),
	puppeteer: { headless: false }
});

//Connection To Database
const sql = require('mssql')
//0const request = new sql.Request();
//request.query('select * from [Notification]') 
const config = {
  user: 'admin',
  password: 'new555',
  database: 'ShahadatApp',
  server: 'WANHOST',
  port:1433,
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 50000
  },
  options: {
    trustServerCertificate: true,
    cryptoCredentialsDetails: {
        minVersion: 'TLSv1'
    }
  }
};
//----------------------------------------

const a2e = s => s.replace(/[٠-٩]/g, d => '٠١٢٣٤٥٦٧٨٩'.indexOf(d));
const morfatPath = 'D:\\morfat\\';



function getAns(lsNum) {
    let newStr = "";
    lsNum.forEach(ele => {
        newStr = newStr.concat(ele).concat('\n ');
    });
    return newStr;
}
async function runMorfat(content, message) {

    try { 
	    
		//let pool = await sql.connect(config);
        const kawmyImage = MessageMedia.fromFilePath(morfatPath.concat(content).concat('.jpg'));
        const personalImage = MessageMedia.fromFilePath(morfatPath.concat(content).concat('_1.jpg'));
        await client.sendMessage(message.from, kawmyImage, { caption: content });
        await client.sendMessage(message.from, personalImage, { caption: content });
		await pool.request().query(`UPDATE [Citizen] SET [MorfkatSent] = 1 WHERE [Kawmy] = '${content}'`);
        console.log('Morfat sent to '.concat(message.from).concat('successfully!'));
		await sleep(2000);
        return "";
    }
    catch (ex) {
		console.log('Morfa2 not exist...');
        return content;
    }
}


async function inQueryTalab (talabNum) {
    try {
       let pool = await sql.connect(config);
       let result = await pool.request().query(`SELECT FullName,TalabNum, Citizen.Kawmy, TalabStatus, [Name], MorfkatNum, ServicePostion FROM Talab INNER JOIN Citizen ON Talab.Kawmy = Citizen.Kawmy INNER JOIN Area ON Area.MktbCode = Talab.RecievedAreaID WHERE TalabNum = '${talabNum}'`);
       
       //sql.close();
       return result.recordset[0];

    }catch(err)
    {
       console.log(err);
       //sql.close();
    }
   
}

async function getNotification () {
    try {
       let pool = await sql.connect(config);
       let result = await pool.request().query(`SELECT [Message] FROM [Notification]`);
       await pool.request().query('DELETE FROM [Notification]')
       return result.recordset;

    }catch(err)
    {
       console.log(err);
       //sql.close();
    }
}
async function SendTmEbla8(){
    try{
        let pool = await sql.connect(config);
        let result = await pool.request().query(`SELECT TalabNum,FullName,Phone,[Name],ServiceType,CertificateType FROM Talab INNER JOIN Citizen ON Talab.Kawmy = Citizen.Kawmy INNER JOIN Area ON Talab.RecievedAreaID = Area.MktbCode WHERE Talab.CreateDate >= CAST( GETDATE()-1 AS Date ) AND Talab.TmEbla8Msg = 0 AND Talab.TalabStatus = 'تم ابلاغه بالدفع'`);
        for(let talab in result.recordset){
            await pool.request().query(`UPDATE [Talab] SET [TmEbla8Msg] = 1 WHERE [TalabNum] = '${result.recordset[talab].TalabNum}'`)
        }
       return result.recordset;
    }catch(err){
        //sql.close();
        console.log(err);
    }
}

async function SendPaid(){
    try{
        let pool = await sql.connect(config);
        let result = await pool.request().query(`SELECT TalabNum,FullName,Phone,[Name],MorfkatNum,ServiceType,CertificateType FROM Talab INNER JOIN Citizen ON Talab.Kawmy = Citizen.Kawmy INNER JOIN Area ON Talab.RecievedAreaID = Area.MktbCode WHERE Talab.CreateDate >= CAST( GETDATE()-1 AS Date ) AND Talab.TmDaf3Msg = 0 AND Talab.TalabStatus = 'تم الدفع'`);
        for(let talab in result.recordset){
            await pool.request().query(`UPDATE [Talab] SET [TmDaf3Msg] = 1 WHERE [TalabNum] = '${result.recordset[talab].TalabNum}'`)
        }
       return result.recordset;

    }catch(err){
        console.log(err);
        //sql.close();
    }
}


async function SendTmEbla8Fawry(){
    try{
        let pool = await sql.connect(config);
        let result = await pool.request().query(`SELECT TalabNum,FullName,Citizen.Kawmy,Phone,[Name],ServiceType,CertificateType FROM Talab INNER JOIN Citizen ON Talab.Kawmy = Citizen.Kawmy INNER JOIN Area ON Talab.RecievedAreaID = Area.MktbCode WHERE Talab.CreateDate >= CAST( GETDATE()-1 AS Date ) AND Talab.TmEbla8Msg = 0 AND Talab.TalabStatus = 'تم ابلاغه فورى'`);
        for(let talab in result.recordset){
            await pool.request().query(`UPDATE [Talab] SET [TmEbla8Msg] = 1 WHERE [TalabNum] = '${result.recordset[talab].TalabNum}'`)
        }

       return result.recordset;
    }catch(err){
        console.log(err);
        //sql.close();
    }

}


async function SendTfawd(){
    try{
        let pool = await sql.connect(config);
        let result = await pool.request().query(`SELECT TalabNum,FullName,Phone FROM Talab INNER JOIN Citizen ON Talab.Kawmy = Citizen.Kawmy WHERE Talab.CreateDate >= CAST( GETDATE()-1 AS Date ) AND isNegotiated = 0 AND Talab.TalabStatus = 'تفاوض'`);
        for(let talab in result.recordset){
            await pool.request().query(`UPDATE [Talab] SET [isNegotiated] = 1 WHERE [TalabNum] = '${result.recordset[talab].TalabNum}'`)
        }

       return result.recordset;

    }catch(err){
        sql.close();
    }
}

async function SendCanceled(){
    try{
        let pool = await sql.connect(config);
        let result = await pool.request().query(`select FullName,Phone, ServiceType,TalabStatus,TalabNum,CreateDate from Citizen inner join Talab on Citizen.Kawmy = Talab.Kawmy where TalabStatus = 'الغاء الطلب' AND CAST(GetDate()-1 as date) = Cast(CreateDate as Date) AND isCanceled = 0`);
        for(let talab in result.recordset){
            await pool.request().query(`UPDATE [Talab] SET [isCanceled] = 1 WHERE [TalabNum] = '${result.recordset[talab].TalabNum}'`)
        }

       return result.recordset;

    }catch(err){
        sql.close();
    }
}

 async function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

const parseExcel = (filename) => {

    const excelData = XLSX.readFile(filename);

    return Object.keys(excelData.Sheets).map(name => ({
        name,
        data: XLSX.utils.sheet_to_json(excelData.Sheets[name]),
    }));
};



client.on('qr', (qr) => {
    qrcode.generate(qr, {small: true});
});




client.on('ready',async () => {
    //intialize connection to data
	pool = await sql.connect(config);

    ////Used to read all chats from Whatsapp
    client.getChats().then(
        chats => { console.log(chats[0])}
        
    );
	
    console.log('Client is ready!');

    //Send Notification
try{
    parallel([
        function () {
        setInterval(function(){ 
            //code goes here that will be run every 3 Minutes.    
            (async () => {
                let Messages = await getNotification();
                try{
                    for(let msg in Messages){
                        if(! Messages[msg].Message.includes('تم إضافة')){
                        var PhoneNum = "2"+Messages[msg].Message.slice(-11) + "@c.us";
                        MsgSent = Messages[msg].Message.slice(0,-11);
                        if(PhoneNum.startsWith('201')){
                        client.sendMessage(PhoneNum, MsgSent);
                        }
                        else{
                            console.log('invalid Phone Number: '+ PhoneNum)
                        }
                        }
                    }
    
                }catch(err){
                    console.log(err);
                }
              })();
        }, 40000)},
        /// Send Tm ebla8 Net
        function () {
        setInterval(function(){ 
            //code goes here that will be run every 3 Minutes.    
            (async () => {
                let Messages = await SendTmEbla8();
                try{
                    let pool = await sql.connect(config);
                    if(Messages != null || Messages != undefined){
                    for(let msg in Messages){
                        var PhoneNum = Messages[msg].Phone.slice(1) + "@c.us";
                        Name = Messages[msg].FullName;
                        Area = Messages[msg].Name.trim();
                        ServiceType = Messages[msg].ServiceType;
                        CertificateType = Messages[msg].CertificateType;
                        MsgSent = 'السيد / '+ Name + "\n"+ 'بخصوص طلبك '+ ServiceType +'\n'+ 'للدفع عن طريق الفيزا او فورى  برجاء الاستعلام عن طريق الرابط الاتى'+'\n'+ 'https://tagned.mod.gov.eg/CertificateSearch.aspx' + '\n\n'+ 'جارى تنفيذ طلبكم برجاء الأنتظار سوف يتم ارسال رسالة لأستلام الخدمة من'+'\n*'+ Area +'*\n'+'عندما تكون الخدمة جاهزة للأستلام' +'\n'
                        if(CertificateType == null || CertificateType == undefined){                       
                            if (ServiceType == 'شهادة'){
                                MsgSent += ' رسوم الشهادة الورقية : 185 جنيهاً ';
                            }
                            else if(ServiceType == 'كارنيه'){
                                MsgSent += 'كارنية مصغر : 185 جنيهاً ';
                            }
                            else if(ServiceType == 'شهادة و كارنيه'){
                                MsgSent += 'رسوم شهادة ورقية + كارنية مصغر : 370 جنيهاً ';
                            }
                        }
                        else if (CertificateType == 'مزدوج الجنسية'){
                            if (ServiceType == 'شهادة'){
                                MsgSent += ' رسوم الشهادة الورقية : 585 جنيهاً ';
                            }
                            else if(ServiceType == 'كارنيه'){
                                MsgSent += 'كارنية مصغر : 585 جنيهاً ';
                            }
                            else if(ServiceType == 'شهادة و كارنيه'){
                                MsgSent += 'رسوم شهادة ورقية + كارنية مصغر : 1170 جنيهاً ';
                            }
                        }
                        MsgSent += '\n'+'إدارة التجنيد و التعبئة'+'\n'+ 'المركز الإلكترونى لخدمات التجنيد'
                        if(PhoneNum.startsWith('201')){
                            client.sendMessage(PhoneNum, MsgSent);
							var UserStatus = await client.isRegisteredUser(PhoneNum);
                            phone = '+' + Messages[msg].Phone.slice(1);
							if(UserStatus)
								await pool.request().query(`UPDATE [Citizen] SET [WhatsAppUser] = 1 where Phone lIKE '${phone}'`);
                        }
                        else{
							console.log('invalid Phone Number: '+ PhoneNum)
                        }
                    }
                }
                }catch(err){
                    console.log(err);
                }
              })();
        }, 30000)},
       /// tm ebl8 Fawry 
        function () {
            setInterval(function(){ 
                //code goes here that will be run every 3 Minutes.    
                (async () => {
                    let Messages = await SendTmEbla8Fawry();
                    try{
                        let pool = await sql.connect(config);
                        if(Messages != null || Messages != undefined){
                        for(let msg in Messages){
                            var PhoneNum = Messages[msg].Phone.slice(1) + "@c.us";
                            Name = Messages[msg].FullName;
                            TalabNum = Messages[msg].TalabNum;
                            Kawmy = Messages[msg].Kawmy;
                            Area = Messages[msg].Name.trim();
                            ServiceType = Messages[msg].ServiceType;
                            CertificateType = Messages[msg].CertificateType;
                            MsgSent = 'السيد / '+ Name +'\n'+' يمكنك الان سداد الرسوم عن طريق ماكينة فورى '+ '\n'+' كود الخدمة : 73602 '+'\n'+' برقم الطلب: ' + TalabNum  +'\n'+' الرقم القومى: '+Kawmy + '\n\n'+ 'جارى تنفيذ طلبكم برجاء الأنتظار سوف يتم ارسال رسالة لأستلام الخدمة من'+'\n*'+ Area +'*\n'+'عندما تكون الخدمة جاهزة للأستلام'+ '\n'
							
                            if(CertificateType == null || CertificateType == undefined){                       
                                if (ServiceType == 'شهادة'){
                                    MsgSent += ' رسوم الشهادة الورقية : 185 جنيهاً ';
                                }
                                else if(ServiceType == 'كارنيه'){
                                    MsgSent += 'كارنية مصغر : 185 جنيهاً ';
                                }
                                else if(ServiceType == 'شهادة و كارنيه'){
                                    MsgSent += 'رسوم شهادة ورقية + كارنية مصغر : 370 جنيهاً ';
                                }
                            }
                            else if (CertificateType == 'مزدوج الجنسية'){
                                if (ServiceType == 'شهادة'){
                                    MsgSent += ' رسوم الشهادة الورقية : 585 جنيهاً ';
                                }
                                else if(ServiceType == 'كارنيه'){
                                    MsgSent += 'كارنية مصغر : 585 جنيهاً ';
                                }
                                else if(ServiceType == 'شهادة و كارنيه'){
                                    MsgSent += 'رسوم شهادة ورقية + كارنية مصغر : 1170 جنيهاً ';
                                }
                            }

                            MsgSent += '\n'+'إدارة التجنيد و التعبئة'+'\n'+ 'المركز الإلكترونى لخدمات التجنيد'
							
                            if(PhoneNum.startsWith('201')){
                                client.sendMessage(PhoneNum, MsgSent);
								var UserStatus =await client.isRegisteredUser(PhoneNum);
                                phone = '+' + Messages[msg].Phone.slice(1);
                                if(UserStatus)
                                    await pool.request().query(`UPDATE [Citizen] SET [WhatsAppUser] = 1 where Phone lIKE '${phone}'`);
                            }
                            else{
                                console.log('invalid Phone Number: '+ PhoneNum)
                            }
                        }
                    }
                    }catch(err){
                        console.log(err);
                    }
                  })();
            }, 30000)},
    
            //send tfaaawd 
            function () {
                setInterval(function(){ 
                    //code goes here that will be run every 4 Minutes.    
                    (async () => {
                        let Messages = await SendTfawd();
                        try{
                            let pool = await sql.connect(config);
                            if(Messages != null || Messages != undefined){
                            for(let msg in Messages){
                                var PhoneNum = Messages[msg].Phone.slice(1) + "@c.us";
                                Name = Messages[msg].FullName;

                                MsgSent ='بخصوص طلبك (شهادة/كارنية) '+'\n' +' اذا كنت تريد الشهادة الورقية فقط قم بارسال الرقم القومي ومكان استلام '+'\n'+'الشهادة . '+' (خدمة كارنية غير متاحة حاليا اونلاين ) '+'\n'+' إدارة التجنيد و التعبئة '+'\n'+' المركز الإلكتروني لخدمات التجنيد'
                                if(PhoneNum.startsWith('201')){
                                    client.sendMessage(PhoneNum, MsgSent);
									var UserStatus = await client.isRegisteredUser(PhoneNum);
                                    phone = '+' + Messages[msg].Phone.slice(1);
                                    if(UserStatus)
                                        await pool.request().query(`UPDATE [Citizen] SET [WhatsAppUser] = 1 where Phone lIKE '${phone}'`);
                                    }
                                    else{
                                        console.log('invalid Phone Number: '+ PhoneNum)
                                    }
                            }
                        }
                        }catch(err){
                            console.log(err);
                        }
                      })();
                }, 40000)}, 

        //Send Paid
        function () {
        setInterval(function(){ 
            //code goes here that will be run every 3 Minutes.    
            (async () => {
                let Messages = await SendPaid();
                try{
                    let pool = await sql.connect(config);
                    if(Messages != null || Messages != undefined){
                    for(let msg in Messages){
                        var PhoneNum = Messages[msg].Phone.slice(1) + "@c.us";
                        Name = Messages[msg].FullName;
                        TalabNum = Messages[msg].TalabNum;
                        Area = Messages[msg].Name.trim();
                        morfkat = Messages[msg].MorfkatNum;
                        ServiceType = Messages[msg].ServiceType;

                        MsgSent = 'السيد / '+ Name + "\n"+ ' لقد قمت بدفع الرسوم المقررة بخصوص طلب '+ ServiceType +' رقم ' + TalabNum + '\n' + 'جارى تنفيذ طلبكم برجاء الأنتظار سوف يتم ارسال رسالة لأستلام الخدمة من'+'\n*'+ Area +'*\n عندما تكون الخدمة جاهزة للأستلام \n'+'\n'+morfkat+'\n'

                        if((ServiceType == 'كارنيه' || ServiceType == 'شهادة و كارنيه') && Area != 'إدارة التجنيد و التعبئة' ){
                            MsgSent+='\n';
                            MsgSent += '*و إرسال الصور مرة أخرى على هذا الرقم*' + '\n';
                        }
                        MsgSent+='\n';
                        MsgSent += 'إدارة التجنيد و التعبئة'+'\n'+ 'المركز الإلكترونى لخدمات التجنيد'
                        if(PhoneNum.startsWith('201')){
                            client.sendMessage(PhoneNum, MsgSent);
							var UserStatus = await client.isRegisteredUser(PhoneNum);
                            phone = '+' + Messages[msg].Phone.slice(1);
							if(UserStatus)
								await pool.request().query(`UPDATE [Citizen] SET [WhatsAppUser] = 1 where Phone lIKE '${phone}'`);
                        }
                        else{
                            console.log('invalid Phone Number: '+ PhoneNum)
                        }
                    }
                }
                }catch(err){
                    console.log(err);
                }
              })();
        }, 30000)},

        //Send Canceled
        function () {
            setInterval(function(){ 
                //code goes here that will be run every 5 Minutes.    
                (async () => {
                    let Messages = await SendCanceled();
                    try{
                        let pool = await sql.connect(config);
                        if(Messages != null || Messages != undefined){
                        for(let msg in Messages){
                            var PhoneNum = Messages[msg].Phone.slice(1) + "@c.us";
                            Name = Messages[msg].FullName;
                            TalabNum = Messages[msg].TalabNum;
                            Kawmy = Messages[msg].Kawmy;
                            ServiceType = Messages[msg].ServiceType;
                            MsgSent = 'السيد / '+ Name + '\n' + 'بخصوص طلب ' + ServiceType + ' رقم طلب: '+ TalabNum + '\n \n' + 'تم إلغاء طلبك لمرور 24 ساعة و عدم سداد الرسوم المقررة' + '\n' + '*لعمل طلب جديد أرسل الرقم 1*' +'\n'+'إدارة التجنيد و التعبئة'+'\n'+ 'المركز الإلكترونى لخدمات التجنيد'
                            if(PhoneNum.startsWith('201')){
                                client.sendMessage(PhoneNum, MsgSent);
								var UserStatus = await client.isRegisteredUser(PhoneNum);
                                phone = '+' + Messages[msg].Phone.slice(1);
                                if(UserStatus)
                                    await pool.request().query(`UPDATE [Citizen] SET [WhatsAppUser] = 1 where Phone lIKE '${phone}'`);
                                }
                                else{
                                    console.log('invalid Phone Number: '+ PhoneNum)
                                }
                        }
                    }
                    }catch(err){
                        console.log(err);
                    }
                  })();
            }, 50000)},
    ]);
    }catch(err){
        console.log(err);
    }
   
});
     
client.on('message',async message => {
		const morfatId = '1611695911';
        const info = await message.getChat();
        if (info.id.user.split('-')[1] === morfatId) {
            let failedLS = [];
            const ls = message.body.split('\n');
			
            for (let i = 0; i < ls.length; i++) {
                let content = ls[i].trim();
                content = a2e(content).trim();
                if (content.length > 0) {
                    const matches = content.match(/(\d*)/g);
                    if (matches !== null && matches.length > 0 && content.length === 14) {
                        let tmsg = await runMorfat(content, message);
						
                        if (tmsg.length > 0) {
                            failedLS.push(tmsg);
                        }
                    }
                    else {
                        failedLS.push(content);
                    }
                }
            }
            if (failedLS.length > 0) {
                await client.sendMessage(message.from, 'القوميات الاتية' + '\n' + "  " + getAns(failedLS) + ' لا توجد لها مرفقات');
            }
            return;
        }
	else{
    // if(message.body.includes('تصريح سفر') || message.body.includes('اذن سفر')) {
		// message.reply('يمكنك تسجيل طلب استخراج تصريح/اذن سفر من خلال الموقع الالكتروني https://tagned.mod.gov.eg/TravelPermissionApplication.aspx#StartContent  أو يمكنك الاتصال على الخدمة الصوتية 15499 واتباع التعليمات');
	// }
    /*if(message.body.includes('سعر') || message.body.includes('تكلفة') || message.body.includes('تتكلف')){
        message.reply('سعر الشهادة الورقية: 80 جنيه')
		message.reply('سعر الكارنيه : 80 جنيه')
		message.reply('سعر الشهادة + الكارنيه : 150 جنيه')
    }*/
    if(message.body === '1' || message.body.charCodeAt(0) === 1633){
        var Msg = `برجاء ارسال الرقم القومى و مكان استلام الخدمة`;
        client.sendMessage(message.from, Msg);
    }
    if(message.body === '2' || message.body.charCodeAt(0) === 1634){
        var Msg = `برجاء ارسال رقم الطلب`;
        client.sendMessage(message.from, Msg);
    }
    if(message.body === '3' || message.body.charCodeAt(0) === 1635){
        var Msg = `برجاء كتابة الشكوى بالتفصيل`;
        client.sendMessage(message.from, Msg);
    }
    /*     if (message.body.includes('استلم') || message.body.includes('أستلم') || message.body.includes('هستلم')){
        message.reply('برجاء ارسال رقم الطلب');
    }*/
    else if(message.body.match(/(\d)/g) && message.body.length==8){
        if (message.body.endsWith('2')){
            var talabNum = message.body.match(/(\d+)/);
            var res = await inQueryTalab(talabNum[0]);
            if (res != null || res != undefined){
            if (res.Name.includes('قسم'))
            {
                res.Name=res.Name+'( مديرية الامن )'
            }
           if(res.TalabStatus.includes('تم ابلاغه فورى')){
                Name = res.FullName;
                TalabNum = res.TalabNum;
                Kawmy = res.Kawmy;
                MsgSent = 'السيد / '+ Name +' يمكنك الان سداد الرسوم عن طريق ماكينة فورى '+'\n'+' برقم الطلب: ' + TalabNum + '\n'+' كود الخدمة : 73602 ' +'\n'+' الرقم القومى: '+Kawmy + '\n\n'+ 'جارى تنفيذ طلبكم برجاء الأنتظار سوف يتم ارسال رسالة لأستلام الخدمة من'+'\n *'+ res.Name +'*\n'+'عندما تكون الخدمة جاهزة للأستلام'+ '\n'+' رسوم الشهادة الورقية : 185 جنيهاً ' + '\n' + 'كارنية مصغر : 185 جنيهاً ' + '\n' + 'رسوم شهادة ورقية + كارنية مصغر : 365 جنيهاً '
                client.sendMessage(message.from, MsgSent);
           }

           else if(res.TalabStatus.includes('تم ابلاغه بالدفع'))
           {
            MsgSent = 'السيد / '+ res.FullName + "\n"+ 'بخصوص طلبك  الشهاده الورقيه'+'\n'+ 'للدفع عن طريق الفيزا او فورى  برجاء الاستعلام عن طريق الرابط الاتى'+'\n'+ 'https://tagned.mod.gov.eg/CertificateSearch.aspx'  + '\n\n'+ 'جارى تنفيذ طلبكم برجاء الأنتظار سوف يتم ارسال رسالة لأستلام الخدمة من'+'\n *'+ res.Name +'*\n'+'عندما تكون الخدمة جاهزة للأستلام'+'\n'+'إدارة التجنيد و التعبئة'+'\n'+ 'المركز الإلكترونى لخدمات التجنيد'
            client.sendMessage(message.from, MsgSent);
           }
           else if(res.TalabStatus == 'تم الدفع' && (res.ServicePostion =='' || res.ServicePostion == null )){
            MsgSent = 'السيد / '+ res.FullName + "\n"+ ' لقد قمت بدفع الرسوم المقررة بخصوص طلب (شهادة) رقم ' + res.TalabNum + '\n\n' + res.MorfkatNum;
            if(res.MorfkatNum == null){
                MsgSent = 'السيد / '+ res.FullName + "\n"+ ' لقد قمت بدفع الرسوم المقررة بخصوص طلب (شهادة) رقم ' + res.TalabNum +'\n'+ 'جارى تنفيذ طلبكم برجاء الأنتظار سوف يتم ارسال رسالة لأستلام الخدمة من'+'\n *'+ res.Name +'*\n'+ '(صورة شخصية خلفية بيضاء + صورة بطاقة الرقم القومى)'+'عندما تكون الخدمة جاهزة للأستلام'
            }
            const cardMessage = "(اذا كان طلبكم خاص بخدمة الكارنية المصغر برجاء ارسال صورة بطاقة الرقم القومى وصورة شخصية على خلفية بيضاء هنا)"
            client.sendMessage(message.from, MsgSent)
            client.sendMessage(message.from, cardMessage);
           }
           else if(res.ServicePostion == 'الخدمة جاهزة للأستلام' || res.ServicePostion == 'تم تسليم الخدمة'){
            MsgSent = 'السيد/ '+ res.FullName + '\n' + 'يمكنك اٍستلام الخدمة من \n'+ '*'+res.Name +'*'+ "\n" + "مواعيد العمل من 7 صباحاً حتى 12 ظهراً \n المركز الألكترونى لخدمات التجنيد \n إدارة التجنيد و التعبئة";
            client.sendMessage(message.from,MsgSent)
        }
           
        }
      
    }
        else{
            msg= 'برجاء كتابة رقم الطلب بطريقة صحيح'
            //message.reply(msg)
			client.sendMessage(message.from, msg);
        }

    }
    /*sql.connect(config, err => {
        const request = new sql.Request()
        request.stream = true // You can set streaming differently for each request
        Messages = request.query('select [Message] from [Notification]');
    
        request.on('rowsaffected', rowCount => {
            // Emitted for each `INSERT`, `UPDATE` or `DELETE` statement
            // Requires NOCOUNT to be OFF (default)
            let result = await pool.request().query(`SELECT [Message] FROM [Notification]`);
            await pool.request().query('DELETE FROM [Notification]');
            sql.close();

            for(let msg in Messages){
                if(! Messages[msg].Message.includes('تم إضافة')){
                var PhoneNum = "2"+Messages[msg].Message.slice(-11) + "@c.us";
                MsgSent = Messages[msg].Message.slice(0,-11);
                client.sendMessage(PhoneNum, MsgSent);
                }
            }
        })
    })*/
    
    if(message.from.slice(0,12) == "201275638989" || message.from.slice(0,12) == "201275626216"){
		
    function ExcelDateToJSDate(serial) {
        var date = new Date(Math.round((serial - 25569) * 86400 * 1000));
     
        return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
     }


    if(message.body.includes('ارسل الخدمات')){
        //GET all Phones From Excel Sheet
        parseExcel("./الخدمات الجاهزة.xlsx").forEach(async element => {
        for(let i = 0; i < element.data.length; i++){
            const chatId = element.data[i].Phone+ "@c.us";
			printDate = ExcelDateToJSDate(element.data[i].PrintDate)
			
            Message = "السيد / "+ element.data[i].Name + "\n"+" بخصوص طلبك "+ "*("+element.data[i].ServiceType+")*"+" يمكنك إستلام الخدمة من \n" + "*"+element.data[i].Place+"*" + "\n" + element.data[i].Locations + "\n" +"تاريخ طباعة : "+ printDate  + "\n" + "مواعيد العمل من 7 صباحاً حتى 12 ظهراً \n المركز الإلكترونى لخدمات التجنيد \n إدارة التجنيد و التعبئة" 
			if(element.data[i].ServiceType == "شهادة"){
				Message += "\n"+ "*"+"فى حالة طلبك (شهادة + كارنيه) انتظر رسالة استلام الكارنيه"+"*";
			}
			
            if(chatId.startsWith('201')){
                client.sendMessage(chatId, Message)
                await sleep(3000);    
            }else{
                console.log('invalid Number: '+chatId);
            }
            
            
        }
        console.log(element.data.length+" Messages Services Ready Sent..")
		client.sendMessage(message.from, 'تم ارسال سحبة التسليمات');
        //message.reply('تم ارسال سحبة التسليمات');

    });
}

   /*if(message.body.includes('ارسل التصاريح')){
    var fs = require('fs');
    var Folder = fs.readdirSync('D:\\bot\\temp');
    for(let i = 0; i < Folder.length; i++){
        const chatId =Folder[0].slice(0,12)+ "@c.us";
        const media = MessageMedia.fromFilePath('D:\\bot\\temp\\'+Folder[i]);
        Message ='مرفق تصريح السفر الإلكتروني الخاص بكم - التصريح موجود حاليا على منظومة الجوزات بجميع منافذ الجمهورية - يقتصر استلام تصريح السفر على استلام النسخه الالكترونيه فقط  - يقدم التصريح الإلكتروني لموظف الجوازات في حالة الطلب فقط - للتواصل معنا أو للاستفسارات (0226339824 - 0226332871 ) أو من خلال الرقم 15499 نحن معك على مدار الاسبوع / 24ساعة - هذا التصريح لا يعفيك من مسئولية التخلف عن التجنيد أو الإستدعاء - إدارة التجنيد و التعبئة تتمنى لكم رحلة موفقة. لاستخراج تصاريح السفر يمكنك زيارة الرابط التالى: https://tagned.mod.gov.eg/18militaryServiceTravelPermissionC.aspx';

        client.sendMessage(chatId,Message);
        client.sendMessage(chatId,media);
        await sleep(3000);
    }
    if(Folder.length == 0){
        message.reply('لا يوجد تصاريح');
    }
    else{
        message.reply('تم ارسال التصاريح');
    }
   }*/
}
	}
});

client.initialize();