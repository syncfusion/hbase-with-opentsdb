package com.syncfusion;

import java.io.IOException;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.hbase.HBaseConfiguration;
import org.apache.hadoop.hbase.HColumnDescriptor;
import org.apache.hadoop.hbase.HTableDescriptor;
import org.apache.hadoop.hbase.client.HBaseAdmin;

public class PreSplitting {
	
	public static void main(String[] args) throws IOException {
        Configuration config = HBaseConfiguration.create();
        HBaseAdmin hbaseAdmin = new HBaseAdmin(config);
		
		//Pass the startKey and endKey as hexa decimal values based on UID generated for metric in TSDB Server	
		byte[] startKey = hexStringToByteArray("000001");
		byte[] endKey = hexStringToByteArray("000004");
 
		//Create tsdb table in HBase. TSDB server will utilize this table for writing the data.
		HTableDescriptor tableDescriptor = new HTableDescriptor("tsdb"); 
		tableDescriptor.addFamily(new HColumnDescriptor("t")); 
        hbaseAdmin.createTable(tableDescriptor, startKey, endKey, 30); 

		System.out.println("tstb Table Created");
    }
	
	//Method to convert string into byte array
	public static byte[] hexStringToByteArray(String uid) {
	    int len = uid.length();
	    byte[] key = new byte[len / 2];
	    for (int i = 0; i < len; i += 2) {
	        key[i / 2] = (byte) ((Character.digit(uid.charAt(i), 16) << 4)
	                             + Character.digit(uid.charAt(i+1), 16));
	    }
	    return key;
	}
}
