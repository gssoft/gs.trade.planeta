% F:\Work\Math\Matlab\Net
% NetDocCell_01.m

R = 3;
C=1;
asmpath = 'D:\VC\1305\gs.trade\GS.Matlab\bin\Debug\';
asmname = 'GS.Matlab.dll';

asm = NET.addAssembly(fullfile(asmpath,asmname));

obj = GS.Matlab.MyGraph;

mlData = cell(obj.getNewData);
%objArr = cell(obj.getObjectArray);
objNewDataArr = obj.getNewDataProp;

figure('Name',char(mlData{1}));
subplot(R,C,1);
% figure('Name',char(mlData{1}));
plot(double(mlData{2}(2)));
xlabel(char(mlData{2}(1)));

subplot(R,C,2);
objArr = obj.getObjectArray();

doubles1 = double(objArr(1));
doubles2 = double(objArr(2));

plot([doubles1 doubles2]);

subplot(R,C,3);
objDouble = obj.getDoubleArray;
doubles = double(objDouble);
plot(doubles);
