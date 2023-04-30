import { Box, Typography } from '@mui/material';
import { QRCodeSVG } from 'qrcode.react';
import { QR_CODE_VALUE_GENERATOR } from '../../staticData';

interface IQrCodeImg {
	qrCodeValue: string;
}

export const QrCodeImg = ({ qrCodeValue, ...props }: IQrCodeImg): JSX.Element => {
	return (
		<Box
			id={`qr-code-${qrCodeValue}`}
			style={{
				display: 'flex',
				border: '2px solid black',
				flexDirection: 'column',
				alignItems: 'center',
				maxWidth: '302px',
			}}
		>
			<Box
				style={{
					border: '2px solid black',
					padding: '15px',
					margin: '15px',
				}}
			>
				<Box
					style={{
						width: '240px',
						height: '240px',
					}}
				>
					<QRCodeSVG size={240} value={QR_CODE_VALUE_GENERATOR(qrCodeValue)} />
				</Box>
			</Box>
			<Typography style={{ fontWeight: 'bold', color: 'black', fontSize: '24px' }}>{qrCodeValue}</Typography>
		</Box>
	);
};
